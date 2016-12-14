<#
 .Synopsis
 Daten abrufen per DataTable
 #>
param([String]$SqlCommand = "Select * From Employees")

$CnString = "Data Source=.\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=Northwind"

try
{
    $Da = New-Object -TypeName System.Data.SqlClient.SqlDataAdapter -ArgumentList $SqlCommand, $CnString
    $Ta = New-Object -TypeName System.Data.DataTable
    $Da.Fill($Ta)
    $Ta
}
catch
{
    Write-Warning "SQL-Fehler ($_)"
}
