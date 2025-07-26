namespace SirCab.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _compressionType = "None";

        [ObservableProperty]
        private List<string> _compressionTypes =
        [
            "None",
            "MSZIP",
            "LZX"
        ];

        [ObservableProperty]
        private string? _destinationDirectory;

        [ObservableProperty]
        private string? _fileName;

        [ObservableProperty]
        private string? _logOut;

        [ObservableProperty]
        private string? _sourceDirectory;

        [ObservableProperty]
        private string? _version = $"{Assembly.GetExecutingAssembly().GetName().Version?.ToString()} - {AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName}";

        [RelayCommand]
        private async Task BrowseDestinationDirectoryAsync()
        {
            TopLevel? topLevel = GetTopLevel();

            if (topLevel?.StorageProvider is { } storageProvider)
            {
                var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Select destination directory",
                    AllowMultiple = false
                });

                if (result.Count > 0)
                    DestinationDirectory = result[0].Path.LocalPath;
            }
        }

        [RelayCommand]
        private async Task BrowseSourceDirectoryAsync()
        {
            TopLevel? topLevel = GetTopLevel();

            if (topLevel?.StorageProvider is { } storageProvider)
            {
                var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Select source directory",
                    AllowMultiple = false
                });

                if (result.Count > 0)
                    SourceDirectory = result[0].Path.LocalPath;
            }
        }

        private static TopLevel? GetTopLevel()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } mainWindow })
                return TopLevel.GetTopLevel(mainWindow);

            return null;
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e) => Log.Error($"(makecab.exe) {e.Data ?? string.Empty}");

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) => Log.Information($"(makecab.exe) {e.Data ?? string.Empty}");

        [RelayCommand]
        private async Task RunAsync()
        {
            try
            {
                Log.Logger = new LoggerConfiguration().CreateLogger();
                IConfigurationService configurationService = new ConfigurationService();
                Configuration configuration = new()
                {
                    SourceDirectory = SourceDirectory,
                    DestinationDirectory = DestinationDirectory,
                    FileName = FileName,
                    CompressionType = Enum.TryParse<CompressionType>(CompressionType, true, out var compressionType) ? compressionType : null
                };
                ISubstService substService = new SubstService();
                IDdfFileService ddfFileService = new DdfFileService(configuration, substService);
                string? ddfFilePath = ddfFileService.Create();

                if (ddfFilePath == null)
                {
                    Log.Error("Ddf file path is null or empty.");

                    return;
                }

                if (!File.Exists(ddfFilePath))
                {
                    Log.Error("Ddf file does not exist.");

                    return;
                }

                ProcessStartInfo startInfo = new()
                {
                    CreateNoWindow = true,
                    FileName = "makecab.exe",
                    Arguments = $"/f {ddfFilePath.WithQuotes()}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                using Process process = new()
                {
                    StartInfo = startInfo
                };

                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.OutputDataReceived += Process_OutputDataReceived;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                substService.Delete(configuration.SourceDirectory!);
                substService.Delete(configuration.DestinationDirectory!);
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(RunAsync));
            }
            finally
            {
                IUpdateService updateService = new UpdateService();

                await updateService.CheckForUpdateAsync();

                Log.CloseAndFlush();
            }
        }
    }
}