@echo off

setlocal enabledelayedexpansion

set /p version=<..\VERSION
set "release_dir=..\out\release"
set "upx_path=..\src\ext\upx-5.0.1-win64\upx.exe"
set "sircab_core_publish_dir=..\out\SirCab.CORE"
set "sircab_cli_publish_dir=..\out\SirCab.CLI"
set "sircab_cli_fat_publish_dir=..\out\SirCab.CLI_fat"
set "sircab_ui_publish_dir=..\out\SirCab.UI"
set "sircab_ui_fat_publish_dir=..\out\SirCab.UI_fat"

echo Cleaning directories...

for %%d in ("%release_dir%" "%sircab_core_publish_dir%" "%sircab_cli_publish_dir%" "%sircab_fat_publish_dir%" "%sircab_ui_publish_dir%" "%sircab_ui_fat_publish_dir%") do (
    if exist "%%d" (
        echo Cleaning %%d...
        rmdir /s /q "%%d"
    )
)

echo Cleaning .cr, .vs, bin and obj directories...

for /r "..\src" %%p in (.cr .vs bin obj) do (
    if exist "%%~p" (
        echo Cleaning "%%~p"...
        rd /s /q "%%~p"
    )
)

echo Restoring SirCab.slnx...

dotnet restore ..\src\SirCab.slnx

if %ERRORLEVEL% neq 0 (
    echo Restore of SirCab.slnx failed.
    exit /b %ERRORLEVEL%
)

echo Checking for outdated packages...

dotnet list ..\src\SirCab.slnx package --outdated

powershell -command "$output = dotnet list ..\src\SirCab.slnx package --outdated --format json 2>$null | ConvertFrom-Json -ErrorAction SilentlyContinue; if ($output.projects.frameworks.topLevelPackages.Count -gt 0) { Write-Host 'Outdated packages found.' -ForegroundColor Red; exit 1 } else { Write-Host 'No outdated packages found.' -ForegroundColor Green }"

if %ERRORLEVEL% neq 0 (
    exit /b %ERRORLEVEL%
)

REM echo Testing SirCab.CORE...

REM dotnet test ..\src\SirCab.CORE.Test\SirCab.CORE.Test.csproj

REM if %ERRORLEVEL% neq 0 (
REM    echo Test of SirCab.CORE failed.
REM    exit /b %ERRORLEVEL%
REM )

echo Building SirCab.CORE...

dotnet publish ..\src\SirCab.CORE\SirCab.CORE.csproj -p:PublishDir="..\%sircab_core_publish_dir%" -p:Version=%version% -c Release

if %ERRORLEVEL% neq 0 (
    echo Build of SirCab.CORE failed.
    exit /b %ERRORLEVEL%
)

del /f /q "%sircab_core_publish_dir%\*.pdb"

echo Archiving SirCab.CORE...

powershell Compress-Archive -Path "%sircab_core_publish_dir%\*" -DestinationPath "%sircab_core_publish_dir%.zip" -Force

if %ERRORLEVEL% neq 0 (
    echo Archiving of SirCab.CORE failed.
    exit /b %ERRORLEVEL%
)

echo Building SirCab.CLI...

dotnet publish ..\src\SirCab.CLI\SirCab.CLI.csproj -p:PublishProfile=FolderProfile -p:PublishDir="..\%sircab_cli_publish_dir%" -p:Version=%version% -c Release

if %ERRORLEVEL% neq 0 (
    echo Build of SirCab.CLI failed.
    exit /b %ERRORLEVEL%
)

del /f /q "%sircab_cli_publish_dir%\*.pdb"

echo Copying SirCab.CLI to SirCab.CLI_fat...

mkdir "%sircab_cli_fat_publish_dir%"

xcopy /e /i /y "%sircab_cli_publish_dir%\*" "%sircab_cli_fat_publish_dir%"

if %ERRORLEVEL% neq 0 (
    echo Copy of SirCab.CLI to SirCab.CLI_fat failed.
    exit /b %ERRORLEVEL%
)

echo Archiving SirCab.CLI_fat...

powershell Compress-Archive -Path "%sircab_cli_fat_publish_dir%\*" -DestinationPath "%sircab_cli_fat_publish_dir%.zip" -Force

if %ERRORLEVEL% neq 0 (
    echo Archiving of SirCab.CLI_fat failed.
    exit /b %ERRORLEVEL%
)

echo Compressing SirCab.CLI...

ren "%sircab_cli_publish_dir%\SirCab.exe" "_SirCab.exe"

"%upx_path%" --best --ultra-brute "%sircab_cli_publish_dir%\_SirCab.exe" -o "%sircab_cli_publish_dir%\SirCab.exe"

if %ERRORLEVEL% neq 0 (
    echo Compression of SirCab.CLI failed.
    exit /b %ERRORLEVEL%
)

"%upx_path%" -t "%sircab_cli_publish_dir%\SirCab.exe"

if %ERRORLEVEL% neq 0 (
    echo Verification of SirCab.CLI compression failed.
    exit /b %ERRORLEVEL%
)

del /f /q "%sircab_cli_publish_dir%\_SirCab.exe"

echo Archiving SirCab.CLI...

powershell Compress-Archive -Path "%sircab_cli_publish_dir%\*" -DestinationPath "%sircab_cli_publish_dir%.zip" -Force

if %ERRORLEVEL% neq 0 (
    echo Archiving of SirCab.CLI failed.
    exit /b %ERRORLEVEL%
)

echo Building SirCab.UI...

dotnet publish ..\src\SirCab.UI\SirCab.UI.csproj -p:PublishProfile=FolderProfile -p:PublishDir="..\%sircab_ui_publish_dir%" -p:Version=%version% -c Release

if %ERRORLEVEL% neq 0 (
    echo Build of SirCab.UI failed.
    exit /b %ERRORLEVEL%
)

del /f /q "%sircab_ui_publish_dir%\*.pdb"

echo Copying SirCab.UI to SirCab.UI_fat...

mkdir "%sircab_ui_fat_publish_dir%"

xcopy /e /i /y "%sircab_ui_publish_dir%\*" "%sircab_ui_fat_publish_dir%"

if %ERRORLEVEL% neq 0 (
    echo Copy of SirCab.UI to SirCab.UI_fat failed.
    exit /b %ERRORLEVEL%
)

echo Archiving SirCab.UI_fat...

powershell Compress-Archive -Path "%sircab_ui_fat_publish_dir%\*" -DestinationPath "%sircab_ui_fat_publish_dir%.zip" -Force

if %ERRORLEVEL% neq 0 (
    echo Archiving of SirCab.UI_fat failed.
    exit /b %ERRORLEVEL%
)

echo Compressing SirCab.UI...

ren "%sircab_ui_publish_dir%\SirCab.exe" "_SirCab.exe"

"%upx_path%" --best --ultra-brute "%sircab_ui_publish_dir%\_SirCab.exe" -o "%sircab_ui_publish_dir%\SirCab.exe"

if %ERRORLEVEL% neq 0 (
    echo Compression of SirCab.UI failed.
    exit /b %ERRORLEVEL%
)

"%upx_path%" -t "%sircab_ui_publish_dir%\SirCab.exe"

if %ERRORLEVEL% neq 0 (
    echo Verification of SirCab.UI compression failed.
    exit /b %ERRORLEVEL%
)

del /f /q "%sircab_ui_publish_dir%\_SirCab.exe"

echo Archiving SirCab.UI...

powershell Compress-Archive -Path "%sircab_ui_publish_dir%\*" -DestinationPath "%sircab_ui_publish_dir%.zip" -Force

if %ERRORLEVEL% neq 0 (
    echo Archiving of SirCab.UI failed.
    exit /b %ERRORLEVEL%
)

mkdir "%release_dir%"

move /y "%sircab_core_publish_dir%.zip" "%release_dir%\SirCab.CORE.zip"
move /y "%sircab_cli_publish_dir%.zip" "%release_dir%\SirCab.CLI.zip"
move /y "%sircab_cli_fat_publish_dir%.zip" "%release_dir%\SirCab.CLI_fat.zip"
move /y "%sircab_ui_publish_dir%.zip" "%release_dir%\SirCab.UI.zip"
move /y "%sircab_ui_fat_publish_dir%.zip" "%release_dir%\SirCab.UI_fat.zip"

endlocal