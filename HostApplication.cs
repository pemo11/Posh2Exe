// File: HostApplication.cs

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

using System.Management.Automation;
using System.Collections.ObjectModel;

namespace Posh2Exe
{
    public class HostApplication
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
        static void Main(string[] args)
        {
            TimeSpan duration;
            DateTime startTime;
            Console.ForegroundColor = ConsoleColor.Green;
            string ps1Path = args[0];
            string exeName = args[1];
            // Wurde nur eine Datei angegeben?
            if (Path.GetPathRoot(ps1Path) == "")
            {
                // Ist nicht zwingend erforderlich - eher der Form halber
                ps1Path = Path.Combine(Environment.CurrentDirectory, ps1Path);
            }
            if (!File.Exists(ps1Path))
            {
                throw new Posh2ExeException(ps1Path + " not found - please check the path again.");
            }

            startTime = DateTime.Now;
            // Create a pipeline for running the script
            using (PowerShell posh = PowerShell.Create())
            {
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                using (StreamReader sr = new StreamReader(curAssembly.GetManifestResourceStream("Posh2Exe.MakeExe.ps1")))
                {
                    string makeExeScript = sr.ReadToEnd();
                    posh.AddScript(makeExeScript);
                    posh.AddParameter("ExeName", exeName);
                    posh.AddParameter("Ps1Path", ps1Path);
                }
                Collection<PSObject> results = posh.Invoke();
            }
            duration = DateTime.Now - startTime;
            Console.WriteLine("*** {0} with embedded {1} created in {2:n2}s", exeName, ps1Path, duration.TotalSeconds);
            Console.ReadLine();
        }
    }
}
