@echo off

setlocal enabledelayedexpansion

set /p version=<..\VERSION
set "release_dir=..\out\release"
set "upx_path=..\src\ext\upx-5.0.1-win64\upx.exe"
set "unicab_publish_dir=..\out\UniCab"
set "unicab_fat_publish_dir=..\out\UniCab_fat"

echo Cleaning directories...

for %%d in ("%release_dir%" "%unicab_publish_dir%" "%unicab_fat_publish_dir%") do (
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

echo Restoring UniCab.sln...

dotnet restore ..\src\UniCab.sln

if %ERRORLEVEL% neq 0 (
    echo Restore of UniCab.sln failed.
    exit /b %ERRORLEVEL%
)

echo Checking for outdated packages...

dotnet list ..\src\UniCab.sln package --outdated

powershell -command "$output = dotnet list ..\src\UniCab.sln package --outdated --format json 2>$null | ConvertFrom-Json -ErrorAction SilentlyContinue; if ($output.projects.frameworks.topLevelPackages.Count -gt 0) { Write-Host 'Outdated packages found.' -ForegroundColor Red; exit 1 } else { Write-Host 'No outdated packages found.' -ForegroundColor Green }"

if %ERRORLEVEL% neq 0 (
    exit /b %ERRORLEVEL%
)

echo Building UniCab...

dotnet publish ..\src\UniCab\UniCab.csproj -p:PublishProfile=FolderProfile -p:PublishDir="..\%unicab_publish_dir%" -p:Version=%version% -c Release

if %ERRORLEVEL% neq 0 (
    echo Build of UniCab failed.
    exit /b %ERRORLEVEL%
)

del /f /q "%unicab_publish_dir%\*.pdb"

echo Copying UniCab to UniCab_fat...

mkdir "%unicab_fat_publish_dir%"

xcopy /e /i /y "%unicab_publish_dir%\*" "%unicab_fat_publish_dir%"

if %ERRORLEVEL% neq 0 (
    echo Copy of UniCab to UniCab_fat failed.
    exit /b %ERRORLEVEL%
)

echo Archiving UniCab_fat...

powershell Compress-Archive -Path "%unicab_fat_publish_dir%\*" -DestinationPath "%unicab_fat_publish_dir%.zip" -Force

if %ERRORLEVEL% neq 0 (
    echo Archiving of UniCab_fat failed.
    exit /b %ERRORLEVEL%
)

echo Compressing UniCab...

ren "%unicab_publish_dir%\UniCab.exe" "_UniCab.exe"

"%upx_path%" --best --ultra-brute "%unicab_publish_dir%\_UniCab.exe" -o "%unicab_publish_dir%\UniCab.exe"

if %ERRORLEVEL% neq 0 (
    echo Compression of UniCab failed.
    exit /b %ERRORLEVEL%
)

"%upx_path%" -t "%unicab_publish_dir%\UniCab.exe"

if %ERRORLEVEL% neq 0 (
    echo Verification of UniCab compression failed.
    exit /b %ERRORLEVEL%
)

del /f /q "%unicab_publish_dir%\_UniCab.exe"

echo Archiving UniCab...

powershell Compress-Archive -Path "%unicab_publish_dir%\*" -DestinationPath "%unicab_publish_dir%.zip" -Force

if %ERRORLEVEL% neq 0 (
    echo Archiving of UniCab failed.
    exit /b %ERRORLEVEL%
)

mkdir "%release_dir%"

move /y "%unicab_publish_dir%.zip" "%release_dir%\UniCab.zip"
move /y "%unicab_fat_publish_dir%.zip" "%release_dir%\UniCab_fat.zip"

endlocal