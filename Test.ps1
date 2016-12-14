# Ein kleines Skript, aus dem eine Exe-Datei gemacht werden soll
param([Long]$LimitMB)
Get-Process | Where WS -GT $LimitMB | Select Name, WS, StartTime, Company | Out-GridView

# Write-Error "Oh, oh - ein Fehler!"
$Ps1Pfad = [Environment]::CurrentDirectory
"Der aktuelle Skriptpfad mit PSScriptRoot: $PSScriptRoot"
$MyInvocation.MyCommand | Select *

"Der aktuelle Skriptpfad mit MyCommand: $($MyInvocation.MyCommand.Path)"
"Der aktuelle Skriptpfad mit CurrentDirectory: $Ps1Pfad"
"Der aktuelle Pfad über die AppDomain: $([System.AppDomain]::CurrentDomain.BaseDirectory.TrimEnd('\'))"

Read-Host -Prompt "Weiter mit der Enter-Taste..."