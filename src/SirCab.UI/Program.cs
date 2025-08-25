namespace SirCab.UI
{
    internal sealed class Program
    {
        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();

        [STAThread]
        public static void Main(string[] args)
        {
            Args = args.Length > 0 ? args : null;

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static string[]? Args { get; set; }
    }
}