// File: PoshHostApplication.cs

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;

namespace Posh2Exe
{
    public class PoshHostApplication
    {
        private bool shouldExit;
        private int exitCode;

        public bool ShouldExit
        {
            get { return this.shouldExit; }
            set { this.shouldExit = value; }
        }

        public int ExitCode
        {
            get { return this.exitCode; }
            set { this.exitCode = value; }
        }

        public static void Main(string[] args)
        {
            // Ps1 file into the Assembly Resource
            Assembly currentAss = Assembly.GetExecutingAssembly();

            // Zur Kontrolle alle Ressourcenamen ausgeben
            foreach (string resName in currentAss.GetManifestResourceNames())
            {
                // Console.WriteLine("*** " + resName);
            }

            // Creating a new PowerShell Host for "executing" the script
            PoshHost poshHost = new PoshHost(new PoshHostApplication());
            poshHost.UI.RawUI.ForegroundColor = ConsoleColor.Green;

            using (Runspace runSpace = RunspaceFactory.CreateRunspace(poshHost))
            {
                runSpace.Open();
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = runSpace;
                    Assembly curAssembly = Assembly.GetExecutingAssembly();
                    // using (StreamReader sr = new StreamReader(curAssembly.GetManifestResourceStream("DBAbfrage1.ps1")))
                    // {
                    // string ps1Script = sr.ReadToEnd();
                    string ps1Script = TextCompress.DecompressStream(curAssembly.GetManifestResourceStream("DBAbfrage1.ps1"));
                    // Console.WriteLine(ps1Script);
                    powershell.AddScript(ps1Script);
                    string pattern = @"(\w+):(.+)";
                    for (int i = 0; i < args.Length; i++)
                    {
                        // Console.WriteLine("*** Arg Nr. {0}: {1}", i, args[i]);
                        if (Regex.IsMatch(args[i], pattern))
                        {
                            Match m = Regex.Match(args[i], pattern);
                            // Console.WriteLine("Arg-Name: {0}, Arg-Value: {1}", m.Groups[1].Value, m.Groups[2].Value);
                            powershell.AddParameter(m.Groups[1].Value, m.Groups[2].Value);
                        }
                        else
                        {
                            powershell.AddParameter(null, args[i]);
                        }
                    }
                    // Pipeline must be written directly to the host window
                    powershell.AddCommand("Out-Host");
                //}
                    // Collect errors
                    powershell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                    Collection<PSObject> results =  powershell.Invoke();
                    if (results.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        foreach(PSObject result in results)
                        {
                            Console.WriteLine("*** {0}", result.ToString());
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                }
            }
        }
    }
}

