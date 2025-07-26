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
        private string? _log;

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
    }
}