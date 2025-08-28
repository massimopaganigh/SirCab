namespace SirCab.CORE.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly HttpClient _httpClient = new();

        public async Task CheckForUpdateAsync()
        {
            try
            {
                Version? currentLocalVersion = Assembly.GetExecutingAssembly().GetName().Version;
                Version? currentRemoteVersion = Version.TryParse(await GetCurrentVersionAsync(), out var parsedVersion) ? parsedVersion : null;

                if (currentLocalVersion == null
                    || currentRemoteVersion == null)
                {
                    Log.Warning("Current local version or current remote version is null or empty.");

                    return;
                }

                if (currentLocalVersion >= currentRemoteVersion)
                    return;

                string manifestUrl = $"https://raw.githubusercontent.com/microsoft/winget-pkgs/refs/heads/master/manifests/s/SirCab/SirCabCLI/{currentRemoteVersion}/SirCab.SirCabCLI.yaml";
                using HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(manifestUrl);

                if (httpResponseMessage.IsSuccessStatusCode)
                    Log.Information("Update available: {0} -> {1}. Run 'winget upgrade SirCab.SirCabCLI'.", currentLocalVersion, currentRemoteVersion);
                else
                    Log.Information("Update available: {0} -> {1}. Download the latest version from 'https://github.com/massimopaganigh/SirCab/releases/latest'.", currentLocalVersion, currentRemoteVersion);
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(CheckForUpdateAsync));
            }
        }

        public async Task DownloadAndInstallUpdateAsync()
        {
            try
            {
                string? currentRemoteVersion = await GetCurrentVersionAsync();

                if (string.IsNullOrEmpty(currentRemoteVersion))
                    return;

                string downloadUrl = $"https://github.com/massimopaganigh/SirCab/releases/download/{currentRemoteVersion}/SirCab.UI.zip";
                string tempPath = Path.GetTempPath();
                string zipFilePath = Path.Combine(tempPath, "SirCab.UI.zip");
                string extractPath = Path.Combine(tempPath, "SirCab_Update");

                using (HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(downloadUrl))
                {
                    httpResponseMessage.EnsureSuccessStatusCode();

                    using FileStream fileStream = new(zipFilePath, FileMode.Create);

                    await httpResponseMessage.Content.CopyToAsync(fileStream);
                }

                if (Directory.Exists(extractPath))
                    Directory.Delete(extractPath, true);

                Directory.CreateDirectory(extractPath);
                ZipFile.ExtractToDirectory(zipFilePath, extractPath);

                string baseDirectory = AppContext.BaseDirectory;
                string batchFileContent = $@"
@echo off
echo Waiting for main application to close...
timeout /t 3 /nobreak > nul

echo Updating application files...
xcopy ""{extractPath}\*"" ""{baseDirectory}"" /E /Y /Q

echo Cleaning up temporary files...
rmdir /s /q ""{extractPath}""
del ""{zipFilePath}""

echo Starting updated application...
start """" ""{Path.Combine(baseDirectory, "SirCab.exe")}""

echo Update completed. This window will close in 3 seconds...
timeout /t 3 /nobreak > nul
del ""%~f0""
";
                string batchFilePath = Path.Combine(tempPath, "update.bat");

                await File.WriteAllTextAsync(batchFilePath, batchFileContent);

                ProcessStartInfo startInfo = new()
                {
                    CreateNoWindow = true,
                    FileName = batchFilePath,
                    //RedirectStandardOutput = true,
                    //RedirectStandardError = true,
                    UseShellExecute = false
                };
                using Process process = new()
                {
                    StartInfo = startInfo
                };

                process.Start();
                Environment.Exit(0);
            }
            catch (Exception) { }
        }

        public async Task<string?> GetCurrentVersionAsync()
        {
            try
            {
                string versionUrl = "https://raw.githubusercontent.com/massimopaganigh/SirCab/refs/heads/main/VERSION";
                string version = await _httpClient.GetStringAsync(versionUrl);

                return version.Trim();
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(GetCurrentVersionAsync));

                return null;
            }
        }
    }
}