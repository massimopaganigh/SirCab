namespace SirCab.CORE.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly HttpClient _httpClient = new();

        private async Task<string?> GetCurrentVersionAsync()
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

        public async Task CheckForUpdateAsync()
        {
            try
            {
                string? currentVersion = await GetCurrentVersionAsync();

                if (string.IsNullOrEmpty(currentVersion))
                {
                    Log.Warning("Current version is null or empty.");

                    return;
                }

                Version? version = Assembly.GetExecutingAssembly().GetName().Version;
                Version? fixedCurrentVersion = Version.Parse(currentVersion);

                if (version == null
                    || fixedCurrentVersion == null)
                {
                    Log.Warning("Version or fixed current version is null or empty.");

                    return;
                }

                if (version >= fixedCurrentVersion)
                    return;

                string manifestUrl = $"https://raw.githubusercontent.com/microsoft/winget-pkgs/refs/heads/master/manifests/s/SirCab/SirCab/{currentVersion}/SirCab.SirCab.yaml";
                using HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(manifestUrl);

                if (httpResponseMessage.IsSuccessStatusCode)
                    Log.Information("Update available: {0}. Run 'winget upgrade SirCab'.", currentVersion);
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(CheckForUpdateAsync));
            }
        }
    }
}