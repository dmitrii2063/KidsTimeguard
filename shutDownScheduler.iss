; Inno Setup Script для приложения Scheduled Shutdown
; Только для Windows 11

[Setup]
; Основная информация
AppId={{8E2D8E2C-5A5C-4E3D-8E2F-5A8C8E2D5A8C}
AppName=Планировщик выключения
AppVersion=1.0
AppVerName=Планировщик выключения v1.0
DefaultDirName={pf}\ShutdownScheduler
DefaultGroupName=Планировщик выключения
AllowNoIcons=yes
OutputDir=installer
OutputBaseFilename=ShutdownScheduler_Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
UninstallDisplayIcon={app}\ScheduledShutdown.exe
UninstallDisplayName=Планировщик выключения

; Указываем минимальную версию Windows 11 (сборка 22000)
MinVersion=10.0.22000

; Только 64-битная архитектура
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "Создать значок на рабочем столе"; GroupDescription: "Дополнительные значки:"
Name: "startupicon"; Description: "Добавить в автозагрузку"; GroupDescription: "Запуск приложения:"

[Files]
Source: "ScheduledShutdown.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "StartupBlocker.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "config.ini"; DestDir: "{app}"; Flags: onlyifdoesntexist

[Icons]
Name: "{group}\Планировщик выключения"; Filename: "{app}\ScheduledShutdown.exe"
Name: "{group}\{cm:UninstallProgram,Планировщик выключения}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Планировщик выключения"; Filename: "{app}\ScheduledShutdown.exe"; Tasks: desktopicon
Name: "{autostartup}\Планировщик выключения"; Filename: "{app}\ScheduledShutdown.exe"; Tasks: startupicon

[Run]
; Запуск после установки
Filename: "{app}\ScheduledShutdown.exe"; Description: "Запустить планировщик"; Flags: postinstall nowait skipifsilent runascurrentuser

; Создание службы блокировки в планировщике
Filename: "schtasks"; Parameters: "/create /tn ""ShutdownBlocker"" /tr ""{app}\StartupBlocker.exe"" /sc onlogon /ru ""SYSTEM"" /f"; Flags: runhidden

[UninstallRun]
; Удаление задач при деинсталляции
Filename: "schtasks"; Parameters: "/delete /tn ""ShutdownBlocker"" /f"; Flags: runhidden

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Registry]
; Добавление в автозагрузку (дополнительно к задаче в планировщике)
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "ShutdownScheduler"; ValueData: """{app}\ScheduledShutdown.exe"""; Flags: uninsdeletevalue; Tasks: startupicon