param(
    [Parameter(Mandatory = $true)]
    [string]$ExePath
)

$ErrorActionPreference = 'Stop'
$taskName = 'KidsComputerTimeGuard\ChildMode'
$exePath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ExePath)

$xml = @"
<?xml version="1.0" encoding="UTF-16"?>
<Task version="1.4" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
  <RegistrationInfo>
    <Description>Kids Computer Time Guard - child mode at logon</Description>
  </RegistrationInfo>
  <Triggers>
    <LogonTrigger>
      <Enabled>true</Enabled>
    </LogonTrigger>
  </Triggers>
  <Principals>
    <Principal id="Author">
      <GroupId>S-1-5-32-545</GroupId>
      <RunLevel>LeastPrivilege</RunLevel>
    </Principal>
  </Principals>
  <Settings>
    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
    <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
    <StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>
    <AllowHardTerminate>true</AllowHardTerminate>
    <StartWhenAvailable>true</StartWhenAvailable>
    <Enabled>true</Enabled>
    <ExecutionTimeLimit>PT0S</ExecutionTimeLimit>
  </Settings>
  <Actions Context="Author">
    <Exec>
      <Command>$([System.Security.SecurityElement]::Escape($exePath))</Command>
      <Arguments>--child</Arguments>
    </Exec>
  </Actions>
</Task>
"@

$xmlFile = Join-Path $env:TEMP 'KidsComputerTimeGuard_task.xml'
[System.IO.File]::WriteAllText($xmlFile, $xml, [System.Text.Encoding]::Unicode)

schtasks /Delete /TN $taskName /F 2>$null | Out-Null
schtasks /Create /TN $taskName /XML $xmlFile /F
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Remove-Item $xmlFile -Force -ErrorAction SilentlyContinue
