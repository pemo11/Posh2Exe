# Posh2Exe
Embeds a ps1 file into an exe so that the script can be executed through the exe. Posh2Exe creates a new exe that embeds a minimal implementation of a PowerShell host that executes the ps1 script. The ps1 file is part of the exe as a resource that consists of compressed text.

Syntax
------

posh2exe test.ps1 test

this command line creates the exe test.exe that embedds the ps1 file test.ps1. The .exe extension can be omitted.

If the ps1 file uses parameters, in the current version the parameters has to be named parameters like so:

.test p1:100

p1 is the name of the parameter, 100 it's value. Its not possible to use the argument only.

Posh2exe has a -q parameter that ommits the header and a short summary.

Limitations
------------
The host is not a "full blown" PowerShell Host so far. Features like Read-Host -AsSecureString, PromptForChoice or Nested Prompts have not been implemented yet. If a script contains one of these elements it will produce an error message when running the script inside the exe.
