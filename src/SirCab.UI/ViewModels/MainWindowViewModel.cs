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
        private string? _fileType = "Cab";

        [ObservableProperty]
        private List<string> _fileTypes =
        [
            "Cab",
            "Wsp",
            "Xsn"
        ];

        [ObservableProperty]
        private string? _footer = $"{File.GetCreationTime(AppContext.BaseDirectory)} - {Environment.OSVersion}";

        [ObservableProperty]
        private bool _isNotRunning = true;

        [ObservableProperty]
        private bool _isNotUpToDate;

        [ObservableProperty]
        private bool _isUpdating = false;

        [ObservableProperty]
        private string? _logOut;

        [ObservableProperty]
        private string? _sourceDirectory;

        [ObservableProperty]
        private string _title = $"SirCab ({Assembly.GetExecutingAssembly().GetName().Version?.ToString()} - {AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName})";

        [ObservableProperty]
        private string? _upToDateText;

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

        [RelayCommand]
        private async Task DownloadAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    IsNotRunning = false;
                    IsUpdating = true;

                    IUpdateService updateService = new UpdateService();

                    await updateService.DownloadAndInstallUpdateAsync();
                }
                catch (Exception) { }
                finally
                {
                    IsUpdating = false;
                    IsNotRunning = true;
                }
            });
        }

        private static TopLevel? GetTopLevel()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } mainWindow })
                return TopLevel.GetTopLevel(mainWindow);

            return null;
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            string errorData = e.Data ?? string.Empty;

            Log.Error($"(makecab.exe) {errorData}");

            if (string.IsNullOrEmpty(errorData))
                return;

            Environment.ExitCode = 1;
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) => Log.Information($"(makecab.exe) {e.Data ?? string.Empty}");

        [RelayCommand]
        private async Task RunAsync() => await RunAsync(null);

        public async Task CheckForUpdatesAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    IUpdateService updateService = new UpdateService();

                    Version? currentLocalVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    Version? currentRemoteVersion = Version.TryParse(await updateService.GetCurrentVersionAsync(), out var parsedVersion) ? parsedVersion : null;

                    if (currentLocalVersion == null
                        || currentRemoteVersion == null)
                        return;

                    if (currentLocalVersion < currentRemoteVersion)
                    {
                        UpToDateText = $"Update available: {currentLocalVersion} -> {currentRemoteVersion}";
                        IsNotUpToDate = true;
                    }
                }
                catch (Exception) { }
            });
        }

        public async Task RunAsync(string[]? args)
        {
            await Task.Run(() =>
            {
                try
                {
                    IsNotRunning = false;
                    LogOut = null;
                    Log.Logger = new LoggerConfiguration().WriteTo.Sink(new UILogEventSink(this)).CreateLogger();
                    IConfigurationService configurationService = new ConfigurationService();
                    Configuration configuration;

                    if (args != null
                        && args.Length > 0)
                    {
                        configuration = configurationService.FromArgs(args);

                        SourceDirectory = configuration.SourceDirectory;
                        DestinationDirectory = configuration.DestinationDirectory;
                        FileName = configuration.FileName;
                        FileType = configuration.FileType?.ToString();
                        CompressionType = configuration.CompressionType?.ToString();
                    }
                    else
                        configuration = new()
                        {
                            SourceDirectory = SourceDirectory,
                            DestinationDirectory = DestinationDirectory,
                            FileName = FileName,
                            FileType = Enum.TryParse<FileType>(FileType, true, out var fileType) ? fileType : null,
                            CompressionType = Enum.TryParse<CompressionType>(CompressionType, true, out var compressionType) ? compressionType : null
                        };

                    ISubstService substService = new SubstService();
                    IDdfFileService ddfFileService = new DdfFileService(configuration, substService);
                    string? ddfFilePath = ddfFileService.Create();

                    if (ddfFilePath == null)
                    {
                        Log.Error("Ddf file path is null or empty.");

                        Environment.ExitCode = 1;

                        return;
                    }

                    if (!File.Exists(ddfFilePath))
                    {
                        Log.Error("Ddf file does not exist.");

                        Environment.ExitCode = 1;

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

                    if (configuration.SubstError == false)
                    {
                        substService.Delete(configuration.SourceDirectory!);
                        substService.Delete(configuration.DestinationDirectory!);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, nameof(RunAsync));

                    Environment.ExitCode = 1;
                }
                finally
                {
                    Log.CloseAndFlush();

                    IsNotRunning = true;

                    if (args != null
                        && args.Length > 0)
                        Environment.Exit(Environment.ExitCode);
                }
            });
        }
    }
}