// ====================================================================
// File: HostApplication.cs
// this is the main file of the application with the main method as 
// the entry point
// version 0.9 - last updated: 12/14/2016
// author: Peter Monadjemi, pm@activetraining.de, poshadmin.de
// ====================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

using System.Management.Automation;
using System.Collections.ObjectModel;

namespace Posh2Exe
{
    /// <summary>
    /// The main application class for creating an exe with an PowerShell host and an embedded script
    /// </summary>
    public class SetupHostApplication
    {
        // if the app runs in quite mode there will be no banner display and no summary
        private static bool quiteMode = false;

        /// <summary>
        /// Just for the fun of it - show a famous quote with every start
        /// </summary>
        /// <returns></returns>
        private static string GetQuote()
        {
            List<string> quotes = new List<string>();
            try
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                // Read text file with famous quotes as resource stream
                using (StreamReader sr = new StreamReader(ass.GetManifestResourceStream("Posh2Exe.Quotes.txt")))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            quotes.Add(line);
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                WriteErrorOutput(ex);
            }
            int z = new Random().Next(0, quotes.Count);
            return quotes[z];
        }

        /// <summary>
        /// Writes an error message to the console
        /// </summary>
        /// <param name="ex"></param>
        private static void WriteErrorOutput(SystemException ex)
        {
            ConsoleColor curColorFor = Console.ForegroundColor;
            ConsoleColor curColorBack = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("Error: {0}", ex.ToString());
            Console.ForegroundColor = curColorFor;
            Console.BackgroundColor = curColorBack;
        }

        /// <summary>
        /// The entry point of this console app (there is another main so beware)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            TimeSpan duration;
            DateTime startTime;

            // save current console color
            ConsoleColor oldColor = Console.ForegroundColor;
            // Green looks more serious
            Console.ForegroundColor = ConsoleColor.Green;
            // check if the first arg is -?
            if (args.Length > 0 && args[0] == "-?")
            {
                Console.WriteLine("usage: Posh2ExeV2 Test.ps1 Test.exe [-q]");
                Environment.Exit(0);
            }
            // check for quiet mode
            if (args.Length == 3 && (args[0] == "-q" || args[0] == "-Q"))
            {
                Console.WriteLine("usage: Posh2ExeV2 Test.ps1 Test.exe [-q]");
                Environment.Exit(0);
            }

            // enough args?
            if (args.Length < 2)
            {
                Console.WriteLine("error: Arguments missing.");
                Console.WriteLine("usage: Posh2ExeV2 Test.ps1 Test.exe [-q]");
                Environment.Exit(-1);
            }
 
            // 1. Argument path of the ps1 file to embed
            string ps1Path = args[0];
            // 2. Argument name of exe file
            string exeName = args[1];

            // Show a little banner
            if (!quiteMode)
            {
                Console.WriteLine();
                Console.WriteLine(new String('*', 80));
                Console.WriteLine("                   Welcome to Posh2Exe v1.0");
                Console.WriteLine("                   I hope the tool serves you well");
                Console.WriteLine(new String('*', 80));
                Console.WriteLine("\nThe quote of the day:\n");
                Console.WriteLine(GetQuote());
                Console.WriteLine();
                Console.WriteLine(new String('*', 80));
            }

            exeName += Path.GetExtension(exeName) != ".exe" ? ".exe" : "";
            // Is the first argument just the file name?
            if (Path.GetPathRoot(ps1Path) == "")
            {
                // Get the full path - not really necessary probably
                ps1Path = Path.Combine(Environment.CurrentDirectory, ps1Path);
            }
            
            // does the file path exist?
            if (!File.Exists(ps1Path))
            {
                throw new Posh2ExeException(ps1Path + " not found - please check the path again.");
            }

            // get the name of the ps1 file
            string ps1Name = Path.GetFileName(ps1Path);

            // time measurement starts
            startTime = DateTime.Now;

            try
            {
                // Prepare the C# compiler
                CompilerParameters compParas = new CompilerParameters();
                compParas.GenerateExecutable = true;
                compParas.OutputAssembly = Path.Combine(Environment.CurrentDirectory, exeName);
                compParas.IncludeDebugInformation = false;

                // create a temp directory
                string tmpDir = Path.Combine(Path.GetTempPath(), "Posh2Exe");
                if (!Directory.Exists(tmpDir))
                {
                    Directory.CreateDirectory(tmpDir);
                }
                compParas.TempFiles = new TempFileCollection(tmpDir);

                // a little compression won't hurt
                string compressedPath = TextCompress.CompressFile(ps1Path);
                string compressedFileName = Path.GetFileName(compressedPath);

                // add the compressed ps1 file as a resource
                compParas.EmbeddedResources.Add(compressedPath);

                // the project uses its own copy of the Powershell assembly
                string autoPath = Path.Combine(Environment.CurrentDirectory, "System.Management.Automation.dll");
                // string autoPath = @"C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Management.Automation\v4.0_3.0.0.0__31bf3856ad364e35\System.Management.Automation.dll";

                // the project uses its own copy of the System.core assembly
                string corePath = Path.Combine(Environment.CurrentDirectory, "System.Core.dll");

                // add reference to some assemblies
                compParas.ReferencedAssemblies.Add(autoPath);
                compParas.ReferencedAssemblies.Add(corePath);
                compParas.ReferencedAssemblies.Add("System.dll");

                CSharpCodeProvider codeProvider = new CSharpCodeProvider();

                // get source files from resources
                Assembly curAss = Assembly.GetExecutingAssembly();
                string[] fileList = new string[]
                {
                "Posh2ExeException.cs",
                "PoshHost.cs",
                "PoshHostApplication.cs",
                "PoshHostRawUserInterface.cs",
                "PoshHostUserInterface.cs",
                "TextCompress.cs"
                };
                string[] sources = new string[fileList.Length];

                string sourceCode = "";
                for (int i = 0; i < fileList.Length; i++)
                {
                    using (StreamReader sr = new StreamReader(curAss.GetManifestResourceStream("Posh2Exe.HostSource." + fileList[i])))
                    {
                        // exchange place holder for resourcename
                        sourceCode = sr.ReadToEnd().Replace("<<resourcename>>", compressedFileName);
                        sources[i] = sourceCode;
                    }
                }
                CompilerResults results = codeProvider.CompileAssemblyFromSource(compParas, sources);

                // show results
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (results.Errors.Count == 0)
                {
                    Console.WriteLine("\n*** Compiled with absolutely no errors.\n");
                    Environment.ExitCode = 0;
                }
                else
                {
                    Console.WriteLine("\n*** Resulting errors:\n");
                    foreach (var error in results.Errors)
                    {
                        Console.WriteLine(error.ToString());
                        Environment.ExitCode = 1;
                    }
                }

                // Delete the compressed file
                File.Delete(compressedPath);

                // measurement stops
                duration = DateTime.Now - startTime;
                // display a summary
                if (!quiteMode)
                {
                    if (results.Errors.Count == 0)
                    {
                        Console.WriteLine("\n*** {0} with embedded {1} created in {2:n2}s\n", exeName, ps1Path, duration.TotalSeconds);
                    }
                    else
                    {
                        Console.WriteLine("\n\n*** {1} could not be embedded in {0}, sorry.\n", exeName, ps1Path);
                    }
                    Console.ForegroundColor = oldColor;
                    Console.WriteLine(new String('*', 80));
                }

            }
            catch (SystemException ex)
            {
                Console.WriteLine("error: {0}", ex);
                Environment.ExitCode = 2;
            }

            // Console.WriteLine("\nJust press one key of your choice on the keyboard");
            // Console.ReadLine();
        }
    }
}
