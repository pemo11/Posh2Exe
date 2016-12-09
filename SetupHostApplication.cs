﻿// ====================================================================
// File: HostApplication.cs
// this is the main file of the application with the main method as 
// the entry point
// version 0.9 - last updated: 12/9/2016
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
    public class SetupHostApplication
    {
        /// <summary>
        /// Just for the fun of it - show a famous quote with every start
        /// </summary>
        /// <returns></returns>
        private static string GetQuote()
        {
            List<string> quotes = new List<string>();
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
            int z = new Random().Next(0, quotes.Count);
            return quotes[z];
        }

        /// <summary>
        /// The entry point of this console app (there is another main so beware)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            TimeSpan duration;
            DateTime startTime;
            // Green looks more serious
            Console.ForegroundColor = ConsoleColor.Green;
            // check if the first arg is -?
            if (args.Length > 0 && args[0] == "-?")
            {
                Console.WriteLine("usage: Posh2ExeV2 Test.ps1 Test.exe");
                Environment.Exit(0);
            }
            // enough args?
            if (args.Length < 2)
            {
                Console.WriteLine("error: Arguments missing.");
                Console.WriteLine("usage: Posh2ExeV2 Test.ps1 Test.exe");
                Environment.Exit(-1);
            }
            // 1. Argument path of the ps1 file to embed
            string ps1Path = args[0];
            // 2. Argument name of exe file
            string exeName = args[1];

            // Show a little banner
            Console.WriteLine();
            Console.WriteLine(new String('*', 80));
            Console.WriteLine("                   Welcome to Posh2Exe v0.9");
            Console.WriteLine("                   I hope the tool serves you well");
            Console.WriteLine(new String('*', 80));
            Console.WriteLine("\nThe quote of the day:\n");
            Console.WriteLine(GetQuote());
            Console.WriteLine();
            Console.WriteLine(new String('*', 80));
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


            // Prepare the C# compiler
            CompilerParameters compParas = new CompilerParameters();
            compParas.GenerateExecutable = true;
            compParas.OutputAssembly = Path.Combine(Environment.CurrentDirectory, exeName);
            compParas.IncludeDebugInformation = true;

            // create a temp directory
            string tmpDir = Path.Combine(Path.GetTempPath(), "Posh2Exe");
            if (!Directory.Exists(tmpDir))
            {
                Directory.CreateDirectory(tmpDir);
            }
            compParas.TempFiles = new TempFileCollection(tmpDir);

            // a little compression won't hurt
            string tmpPath = TextCompress.CompressFile(ps1Path);
            string tmpPs1Path = Path.Combine(tmpDir, ps1Name);

            // move the ps1 file temporarily so that the compressed file can added as resource file
            File.Copy(ps1Path, tmpPs1Path, true);
            File.Copy(tmpPath, ps1Path, true);

            // add the compressed ps1 file as a resource
            compParas.EmbeddedResources.Add(ps1Path);
            // add reference to some assemblies
            // Ältere System.Management.automation.dll
            // string autoPath = Path.Combine(Environment.CurrentDirectory, "System.Management.Automation.dll");
            string autoPath = @"C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Management.Automation\v4.0_3.0.0.0__31bf3856ad364e35\System.Management.Automation.dll";
            // TODO: Nicht ganz optimal
            string corePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Core.dll";
            compParas.ReferencedAssemblies.Add(autoPath);
            compParas.ReferencedAssemblies.Add(corePath);
            compParas.ReferencedAssemblies.Add("System.dll");

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            // exchange place holder for resourcename
            string sourceCode = "";
            File.Copy(@"HostSource\PoshHostApplication.cs", @"HostSource\PoshHostApplicationEx.cs", true);

            // read source file for exchanging the placeholder with the name of the ps1 file
            using (StreamReader sr = new StreamReader(@"HostSource\PoshHostApplicationEx.cs"))
            {
                sourceCode = sr.ReadToEnd();
                sourceCode = sourceCode.Replace("<<resourcename>>", ps1Name);
            }
            // save the source file with the ps1 file name
            using (StreamWriter sw = new StreamWriter(@"HostSource\PoshHostApplicationEx.cs"))
            {
                sw.WriteLine(sourceCode);
            }

            // compile all source files into a exe
            CompilerResults results = codeProvider.CompileAssemblyFromFile(compParas,
                    @"HostSource\PoshHostApplicationEx.cs",
                    @"HostSource\PoshHost.cs",
                    @"HostSource\PoshHostRawUserInterface.cs",
                    @"HostSource\PoshHostUserInterface.cs",
                    @"HostSource\Posh2ExeException.cs",
                    @"HostSource\TextCompress.cs");

            // show results
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (results.Errors.Count == 0)
            {
                Console.WriteLine("*** Compiled with no errors.");
            }
            else
            { 
                Console.WriteLine("*** Resulting errors:");
                foreach(var error in results.Errors)
                {
                    Console.WriteLine(error.ToString());
                }
            }

            // get the uncompressed ps1 file back
            File.Copy(tmpPs1Path, ps1Path, true);

            // measurement stops
            duration = DateTime.Now - startTime;
            // display a summary
            Console.WriteLine("*** {0} with embedded {1} created in {2:n2}s", exeName, ps1Path, duration.TotalSeconds);
            Console.WriteLine(new String('*', 80));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nJust press one key of your choice on the keyboard");
            Console.ReadLine();
        }
    }
}
