Import-Module -Name ReadNamedPipe

$id = $args[0]

Write-Host "Opening ipc channel with id $id"

$msg = Read-NamedPipe -PipeName "sshmancom-$id" -Timeout 5000
Write-Host "Received ipc: $msg; Connecting..."
ssh $msg
exit $LASTEXITCODE