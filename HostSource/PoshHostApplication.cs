// ====================================================================
// File: PoshHostApplication.cs
// the bare minimum implementation of a PowerShell Host that runs the
// script that is embedded as a resource
// ====================================================================

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;

namespace Posh2Exe
{
    /// <summary>
    /// The Posh Host class that will be embedded into the exe file
    /// </summary>
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

        /// <summary>
        /// The entry point for the Posh Host application that runs the embedded script
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Ps1 file into the Assembly Resource
            Assembly currentAss = Assembly.GetExecutingAssembly();

            // Only for testing purpose - display the name of all resources
            foreach (string resName in currentAss.GetManifestResourceNames())
            {
                // Console.WriteLine("*** " + resName);
            }

            // Creating a new PowerShell Host for "executing" the script
            PoshHost poshHost = new PoshHost(new PoshHostApplication());
            poshHost.UI.RawUI.ForegroundColor = ConsoleColor.Green;

            using (Runspace runSpace = RunspaceFactory.CreateRunspace(poshHost))
            {
                runSpace.ApartmentState = System.Threading.ApartmentState.STA;
                runSpace.Open();
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = runSpace;
                    Assembly curAssembly = Assembly.GetExecutingAssembly();
                    // remember, the ps1 file is compressed so a litte decompression is necessary
                    string ps1Script = TextCompress.DecompressStream(curAssembly.GetManifestResourceStream("<<resourcename>>"));
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
                    // the output will be text not objects :(
                    powershell.AddCommand("Out-Host");
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
