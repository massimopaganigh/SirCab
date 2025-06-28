@echo off

setlocal enabledelayedexpansion

set /p version=<..\VERSION
set "release_dir=..\out\release"
set "upx_path=..\src\ext\upx-5.0.1-win64\upx.exe"
set "sircab_publish_dir=..\out\SirCab"
set "sircab_fat_publish_dir=..\out\SirCab_fat"

echo Cleaning directories...

for %%d in ("%release_dir%" "%sircab_publish_dir%" "%sircab_fat_publish_dir%") do (
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

echo Restoring SirCab.sln...

dotnet restore ..\src\SirCab.sln

if %ERRORLEVEL% neq 0 (
    echo Restore of SirCab.sln failed.
    exit /b %ERRORLEVEL%
)

echo Checking for outdated packages...

dotnet list ..\src\SirCab.sln package --outdated

powershell -command "$output = dotnet list ..\src\SirCab.sln package --outdated --format json 2>$null | ConvertFrom-Json -ErrorAction SilentlyContinue; if ($output.projects.frameworks.topLevelPackages.Count -gt 0) { Write-Host 'Outdated packages found.' -ForegroundColor Red; exit 1 } else { Write-Host 'No outdated packages found.' -ForegroundColor Green }"

if %ERRORLEVEL% neq 0 (
    exit /b %ERRORLEVEL%
)

echo Building SirCab...

dotnet publish ..\src\SirCab\SirCab.csproj -p:PublishProfile=FolderProfile -p:PublishDir="..\%sircab_publish_dir%" -p:Version=%version% -c Release

if %ERRORLEVEL% neq 0 (
    echo Build of SirCab failed.
    exit /b %ERRORLEVEL%
)

del /f /q "%sircab_publish_dir%\*.pdb"

echo Copying SirCab to SirCab_fat...

mkdir "%sircab_fat_publish_dir%"

xcopy /e /i /y "%sircab_publish_dir%\*" "%sircab_fat_publish_dir%"

if %ERRORLEVEL% neq 0 (
    echo Copy of SirCab to SirCab_fat failed.
    exit /b %ERRORLEVEL%
)

echo Archiving SirCab_fat...

powershell Compress-Archive -Path "%sircab_fat_publish_dir%\*" -DestinationPath "%sircab_fat_publish_dir%.zip" -Force

if %ERRORLEVEL% neq 0 (
    echo Archiving of SirCab_fat failed.
    exit /b %ERRORLEVEL%
)

echo Compressing SirCab...

ren "%sircab_publish_dir%\SirCab.exe" "_SirCab.exe"

"%upx_path%" --best --ultra-brute "%sircab_publish_dir%\_SirCab.exe" -o "%sircab_publish_dir%\SirCab.exe"

if %ERRORLEVEL% neq 0 (
    echo Compression of SirCab failed.
    exit /b %ERRORLEVEL%
)

"%upx_path%" -t "%sircab_publish_dir%\SirCab.exe"

if %ERRORLEVEL% neq 0 (
    echo Verification of SirCab compression failed.
    exit /b %ERRORLEVEL%
)

del /f /q "%sircab_publish_dir%\_SirCab.exe"

echo Archiving SirCab...

powershell Compress-Archive -Path "%sircab_publish_dir%\*" -DestinationPath "%sircab_publish_dir%.zip" -Force

if %ERRORLEVEL% neq 0 (
    echo Archiving of SirCab failed.
    exit /b %ERRORLEVEL%
)

mkdir "%release_dir%"

move /y "%sircab_publish_dir%.zip" "%release_dir%\SirCab.zip"
move /y "%sircab_fat_publish_dir%.zip" "%release_dir%\SirCab_fat.zip"

endlocal