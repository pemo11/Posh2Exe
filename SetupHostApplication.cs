// File: HostApplication.cs

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
        static void Main(string[] args)
        {
            TimeSpan duration;
            DateTime startTime;
            Console.ForegroundColor = ConsoleColor.Green;
            string ps1Path = args[0];
            string exeName = args[1];
            exeName += Path.GetExtension(exeName) != ".exe" ? ".exe" : "";
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

            string ps1Name = Path.GetFileName(ps1Path);

            startTime = DateTime.Now;

            CompilerParameters compParas = new CompilerParameters();
            compParas.GenerateExecutable = true;
            compParas.OutputAssembly = Path.Combine(Environment.CurrentDirectory, exeName);
            compParas.IncludeDebugInformation = true;

            string tmpDir = Path.Combine(Path.GetTempPath(), "Posh2Exe");
            if (!Directory.Exists(tmpDir))
            {
                Directory.CreateDirectory(tmpDir);
            }
            compParas.TempFiles = new TempFileCollection(tmpDir);

            // Ps1-Datei komprimieren
            string tmpPath = TextCompress.CompressFile(ps1Path);
            string tmpPs1Path = Path.Combine(tmpDir, ps1Name);

            // Ps1-Datei vorübergehend verschieben
            File.Copy(ps1Path, tmpPs1Path, true);
            File.Copy(tmpPath, ps1Path, true);

            compParas.EmbeddedResources.Add(ps1Path);
            string autoPath = Path.Combine(Environment.CurrentDirectory, "System.Management.Automation.dll");
            compParas.ReferencedAssemblies.Add(autoPath);
            compParas.ReferencedAssemblies.Add("System.dll");

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            // exchange place holder for resourcename
            string sourceCode = "";
            File.Copy(@"..\..\PoshHostApplication.cs", @"..\..\PoshHostApplicationEx.cs", true);

            using (StreamReader sr = new StreamReader(@"..\..\PoshHostApplicationEx.cs"))
            {
                sourceCode = sr.ReadToEnd();
                sourceCode = sourceCode.Replace("<<resourcename>>", ps1Name);
            }
            using (StreamWriter sw = new StreamWriter(@"..\..\PoshHostApplicationEx.cs"))
            {
                sw.WriteLine(sourceCode);
            }

            CompilerResults results = codeProvider.CompileAssemblyFromFile(compParas,
                    @"..\..\PoshHostApplicationEx.cs",
                    @"..\..\PoshHost.cs",
                    @"..\..\PoshHostRawUserInterface.cs",
                    @"..\..\PoshHostUserInterface.cs",
                    @"..\..\Posh2ExeException.cs",
                    @"..\..\TextCompress.cs");

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

            // Ps1-Datei wieder zurückkopieren
            File.Copy(tmpPs1Path, ps1Path, true);

            duration = DateTime.Now - startTime;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("*** {0} with embedded {1} created in {2:n2}s", exeName, ps1Path, duration.TotalSeconds);
            Console.ReadLine();
        }
    }
}
