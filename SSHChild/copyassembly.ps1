$target = "~/Documents/PowerShell/Modules/ReadNamedPipe/ReadNamedPipeCmdlet.dll"
if([System.IO.File]::Exists($target)) {
    Remove-Item $target
}
Copy-Item "$PWD/publish/ReadNamedPipeCmdlet.dll" $target