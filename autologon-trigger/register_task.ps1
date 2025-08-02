$exePath = "C:\Path\To\autologon-trigger.exe"

$action = New-ScheduledTaskAction -Execute $exePath
$trigger = New-ScheduledTaskTrigger -AtStartup
$principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -RunLevel Highest

Register-ScheduledTask -TaskName "AutologonTrigger" -Action $action -Trigger $trigger -Principal $principal

Write-Host "Scheduled task 'AutologonTrigger' created to run at system startup."
