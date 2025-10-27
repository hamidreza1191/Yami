; Inno Setup script to package ScaffGuard after publishing self-contained
#define MyAppName "ScaffGuard"
#define MyAppVersion "0.1.0"
#define MyAppPublisher "You"
#define MyAppExeName "ScaffGuard.exe"

[Setup]
AppId={{F1B0F6D2-66A1-4CF7-9D93-4D7B9F5C2C8F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=ScaffGuard-Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "farsi"; MessagesFile: "compiler:Languages\Farsi.isl"

[Tasks]
Name: "desktopicon"; Description: "ایجاد میانبر دسکتاپ"; GroupDescription: "میانبرها:"; Flags: unchecked

[Files]
; Update this Source with your publish directory path
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "اجرای برنامه"; Flags: nowait postinstall skipifsilent
