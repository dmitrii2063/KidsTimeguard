; Inno Setup — Kids Computer Time Guard
; 1) dotnet publish -c Release -r win-x64 --self-contained false
; 2) Открыть этот .iss в Inno Setup Compiler и собрать

#define MyAppName "Kids Computer Time Guard"
#define MyAppVersion "1.0.1"
#define MyAppPublisher "Family"
#define MyAppExeName "KidsComputerTimeGuard.exe"
#define PublishDir "bin\Release\net8.0-windows\win-x64\publish"

[Setup]
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\KidsComputerTimeGuard
DefaultGroupName={#MyAppName}
OutputDir=installer
OutputBaseFilename=KidsComputerTimeGuard_Setup
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "Создать ярлык «Родитель» на рабочем столе"; GroupDescription: "Дополнительно:"; Flags: checkedonce

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "installer\RegisterChildTask.ps1"; DestDir: "{app}\installer"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName} (родитель)"; Filename: "{app}\{#MyAppExeName}"; Parameters: "--parent"; Comment: "Требуется пароль"
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Parameters: "--child"
Name: "{autodesktop}\{#MyAppName} (родитель)"; Filename: "{app}\{#MyAppExeName}"; Parameters: "--parent"; Tasks: desktopicon

[Run]
Filename: "powershell.exe"; Parameters: "-NoProfile -ExecutionPolicy Bypass -File ""{app}\installer\RegisterChildTask.ps1"" -ExePath ""{app}\{#MyAppExeName}"""; StatusMsg: "Создание задания Планировщика…"; Flags: runhidden waituntilterminated
Filename: "{app}\{#MyAppExeName}"; Parameters: "--parent"; Description: "Открыть настройки родителя (задать пароль)"; Flags: postinstall nowait skipifsilent

[UninstallRun]
Filename: "schtasks.exe"; Parameters: "/Delete /TN ""KidsComputerTimeGuard\ChildMode"" /F"; Flags: runhidden; RunOnceId: "DelTask"

[UninstallDelete]
Type: filesandordirs; Name: "{commonappdata}\KidsComputerTimeGuard"
