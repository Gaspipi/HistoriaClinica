#define AppName "Historia Clínica"
#define AppPublisher "Gaspar Ferreyra"
#define AppUrl "https://github.com/Gaspipi/HistoriaClinica"
#define AppExeName "HistoriaClinica.exe"

#ifndef AppVersion
  #define AppVersion "2.0.1"
#endif

[Setup]
AppId={{e4bc6c3c-b7d4-4122-9589-9e3bba65a1c8}}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppUrl}
AppSupportURL={#AppUrl}/issues
AppUpdatesURL={#AppUrl}/releases
DefaultDirName={localappdata}\Programs\HistoriaClinica
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
DisableDirPage=auto
UsePreviousAppDir=yes
PrivilegesRequired=lowest
OutputDir=..\artifacts\installer
OutputBaseFilename=HistoriaClinica-{#AppVersion}-setup
SetupIconFile=..\src\HistoriaClinica.App\Assets\Icons\historia-clinica.ico
UninstallDisplayIcon={app}\app\{#AppVersion}\{#AppExeName}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0
LicenseFile=..\LICENSE
CloseApplications=no
RestartApplications=no
VersionInfoVersion={#AppVersion}
VersionInfoCompany={#AppPublisher}
VersionInfoDescription={#AppName} installer
VersionInfoProductName={#AppName}
VersionInfoProductVersion={#AppVersion}
VersionInfoCopyright=Copyright © 2022-2026 {#AppPublisher}

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\artifacts\publish\win-x64\*"; DestDir: "{app}\app\{#AppVersion}"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\app\{#AppVersion}\{#AppExeName}"; WorkingDir: "{app}\app\{#AppVersion}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\app\{#AppVersion}\{#AppExeName}"; WorkingDir: "{app}\app\{#AppVersion}"; Check: ShouldCreateDesktopIcon

[Run]
Filename: "{app}\app\{#AppVersion}\{#AppExeName}"; Description: "Abrir {#AppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}\app"

[Code]
function ShouldCreateDesktopIcon: Boolean;
begin
  Result := WizardIsTaskSelected('desktopicon')
    or FileExists(ExpandConstant('{autodesktop}\{#AppName}.lnk'));
end;

function IsAppRunning: Boolean;
begin
  Result := FindWindowByWindowName('Historia Clínica') <> 0;
end;

function DatabasePath: String;
begin
  Result := GetEnv('HISTORIA_DB_PATH');
  if Result = '' then
    Result := ExpandConstant('{userdocs}\Historia.db')
  else
    Result := ExpandFileName(Result);
end;

function BackupDatabase: String;
var
  SourcePath, BackupDir, BackupPath: String;
begin
  Result := '';
  SourcePath := DatabasePath;
  if not FileExists(SourcePath) then
    Exit;

  BackupDir := AddBackslash(ExtractFileDir(SourcePath)) + 'Backups';
  if not ForceDirectories(BackupDir) then
  begin
    Result := 'No se pudo crear la carpeta de backups en ' + BackupDir + '.';
    Exit;
  end;

  BackupPath := AddBackslash(BackupDir)
    + 'pre-install-v{#AppVersion}-'
    + GetDateTimeString('yyyymmddhhnnss', #0, #0)
    + '.db';

  if not CopyFile(SourcePath, BackupPath, False) then
  begin
    Result := 'No se pudo crear el backup de la base de datos en ' + BackupPath + '.';
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  if IsAppRunning then
  begin
    Result := 'Cerrá Historia Clínica antes de instalar o actualizar la aplicación.';
    Exit;
  end;

  Result := BackupDatabase;
end;
