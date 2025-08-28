@echo off

setlocal enabledelayedexpansion

set "sybau=%1"
set "wingetcreate_path=..\src\ext\Microsoft.WindowsPackageManagerManifestCreator_1.9.14.0\wingetcreate.exe"

for /f "usebackq delims=" %%i in ("..\VERSION") do set "version_raw=%%i"

echo Releasing SirCab.SirCabCLI...

%wingetcreate_path% update --submit --token "%sybau%" --urls "https://github.com/massimopaganigh/SirCab/releases/download/%version_raw%/SirCab.CLI_fat.zip" --version %version_raw% SirCab.SirCabCLI

if %ERRORLEVEL% neq 0 (
    echo Release of SirCab.SirCabCLI failed.
    exit /b %ERRORLEVEL%
)

echo Releasing SirCab.SirCabUI...

%wingetcreate_path% update --submit --token "%sybau%" --urls "https://github.com/massimopaganigh/SirCab/releases/download/%version_raw%/SirCab.UI_fat.zip" --version %version_raw% SirCab.SirCabUI

if %ERRORLEVEL% neq 0 (
    echo Release of SirCab.SirCabUI failed.
    exit /b %ERRORLEVEL%
)

endlocal