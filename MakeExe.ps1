<#
 .Synopsis
 Creates a Console Application with a PowerShell Host
#>

param([String]$Ps1Name="Test.ps1", [String]$ExeName="PsTest", [Hashtable]$ScriptParas)

# Just needed for the MessageBox class
Add-Type -AssemblyName PresentationFramework

# if no extension add one
if ([IO.Path]::GetExtension($ExeName) -ne ".exe")
{
	$ExeName += ".exe"
}

# build the complete path
$ExePath = Join-Path -Path ([Environment]::CurrentDirectory) -ChildPath $ExeName
$Ps1Path = Resolve-Path -Path $Ps1Name

# The complete C# source code for a PowerShell host application with a minimum of 
# functionality required for executing a PowerShell script and displaying its output

$CSCode = @"
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Globalization;
using System.Collections.ObjectModel;

namespace PemoHost
{
    <*PoshHost.cs*>

    <*PoshHostRawUserInterface.cs*>

    <*PoshHostUserInterface.cs*>

    <*Posh2ExeException.cs*>


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

        public static void Main(string[] args)
        {
		    // Ps1 file into the Assembly Resource
		    Assembly currentAss = Assembly.GetExecutingAssembly();

            // Zur Kontrolle alle Ressourcenamen ausgeben
            foreach(string resName in currentAss.GetManifestResourceNames())
            {
                // Console.WriteLine("*** " + resName);
            }

		    // Creating a new PowerShell Host for "executing" the script
		    PoshHost poshHost = new PoshHost(new HostApplication());
            poshHost.UI.RawUI.ForegroundColor = ConsoleColor.Green;

            using (Runspace runSpace = RunspaceFactory.CreateRunspace(poshHost))
            {
                runSpace.Open();
		        using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = runSpace;
                    Assembly curAssembly = Assembly.GetExecutingAssembly();
                    using (StreamReader sr = new StreamReader(curAssembly.GetManifestResourceStream("Test.ps1")))
				    {
                        string ps1Script = sr.ReadToEnd();
						// TODO: Ressource dekomprimieren über System.IO.Compression
                        Console.WriteLine(ps1Script);
                        powershell.AddScript(ps1Script);
						string pattern = @"(\w+):(\w+)";
						for(int i = 0; i < args.Length; i++)
						{
							// Console.WriteLine("*** Arg Nr. {0}: {1}", i, args[i]);
			                if (Regex.IsMatch(args[i], pattern))
						    {
								Match m = Regex.Match(args[i], pattern);
								powershell.AddParameter(m.Groups[1].Value, m.Groups[2].Value);
							}
						}
                        powershell.AddCommand("Out-Default");
                    }
                    // Collect errors
                    powershell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                    powershell.Invoke();
                }
            }
	    }
    }
}
"@

 # [System.CodeDom.Compiler.CompilerParameters].GetConstructors().ForEach{ $_.GetParameters() | Select Name }
 # "/r:System.Management.Automation.dll /res:$Ps1Path"

# Include all the source code files in the Posh subdirectory
$FilePattern = "<\*(\w+.cs)\*>"

$Matches = Select-String -InputObject $CSCode -Pattern $FilePattern -AllMatches | Select -Expand Matches 

$SB = New-Object -TypeName System.Text.StringBuilder

$Matches | ForEach {
	 # $Pfad = Join-Path -Path $PSScriptRoot -ChildPath $_.Groups[1].Value
	 $BasePath = "$([Environment]::CurrentDirectory)\..\..\Posh"
	 $Pfad = Join-Path -Path $BasePath -ChildPath $_.Groups[1].Value
	 [void]$SB.Append((Get-Content -Path $Pfad -Raw))
	 # [void]$SB.Append("`n")
}

# Replace place holders with the source code

$CSCodeNew = $CSCode.Substring(0, $Matches[0].Index)
$CSCodeNew += $SB.ToString()

$CSCodeNew += $CSCode.Substring($Matches[-1].Index + $Matches[-1].Length)

$CompParas = New-Object -TypeName System.CodeDom.Compiler.CompilerParameters

$PsAutoPath = Join-Path -Path ([Environment]::CurrentDirectory) -ChildPath "System.Management.Automation.dll"
[void]$CompParas.ReferencedAssemblies.Add($PsAutoPath)
[void]$CompParas.ReferencedAssemblies.Add("System.dll")

// TODO: Ressource komprimieren über System.IO.Compression

[void]$CompParas.EmbeddedResources.Add($Ps1Path)
$CompParas.OutputAssembly = $ExePath
$CompParas.GenerateExecutable = $true

# [System.Windows.Messagebox]::Show($CsCodeNew)
Add-Type -TypeDefinition $CSCodeNew -CompilerParameters $CompParas