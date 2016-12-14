# just a simple script that can be embed into an exe
param([Long]$LimitMB)
Get-Process | Where WS -GT $LimitMB | Select Name, WS, StartTime, Company | Out-GridView

# Write-Error "Oh, oh - error!"
$Ps1Path = [Environment]::CurrentDirectory
"The current path of the script with PSScriptRoot: $PSScriptRoot"
$MyInvocation.MyCommand | Select *

"The current path of the script with: $($MyInvocation.MyCommand.Path)"
"The current path of the script with: $Ps1Path"
"The current path of the script with the AppDomain: $([System.AppDomain]::CurrentDomain.BaseDirectory.TrimEnd('\'))"

Read-Host -Prompt "Go on with the any key..."