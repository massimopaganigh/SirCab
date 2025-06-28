@echo off

setlocal enabledelayedexpansion

set "sybau=%1"
set "wingetcreate_path=..\src\ext\Microsoft.WindowsPackageManagerManifestCreator_1.9.14.0\wingetcreate.exe"

for /f "usebackq delims=" %%i in ("..\VERSION") do set "version_raw=%%i"

echo Releasing SirCab.SirCab...

%wingetcreate_path% update --submit --token "%sybau%" --urls "https://github.com/massimopaganigh/SirCab/releases/download/%version_raw%/SirCab_fat.zip" --version %version_raw% SirCab.SirCab

if %ERRORLEVEL% neq 0 (
    echo Release of SirCab.SirCab failed.
    exit /b %ERRORLEVEL%
)

endlocal