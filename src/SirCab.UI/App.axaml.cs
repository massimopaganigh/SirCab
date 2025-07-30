namespace SirCab.UI
{
    public partial class App : Application
    {
        private void DisableAvaloniaDataAnnotationValidation()
        {
            DataAnnotationsValidationPlugin[] dataValidationPluginsToRemove = [.. BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>()];

            foreach (DataAnnotationsValidationPlugin plugin in dataValidationPluginsToRemove)
                BindingPlugins.DataValidators.Remove(plugin);
        }

        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DisableAvaloniaDataAnnotationValidation();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}