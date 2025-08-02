$source = "C:\Path\To\clear-autologon.exe"
$startupFolder = [Environment]::GetFolderPath("Startup")
$destination = Join-Path $startupFolder "autologon-cleanup.exe"

Copy-Item -Path $source -Destination $destination -Force
Write-Host "Copied clear-autologon.exe to startup folder."
