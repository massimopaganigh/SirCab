namespace UniCab
{
    internal class Program
    {
        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) => Log.Information($"(makecab.exe) {e.Data}" ?? string.Empty);

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log.Error($"(makecab.exe) {e.Data}" ?? string.Empty);

            Environment.ExitCode = 1;
        }

        public static void Main(string[] args)
        {
            try
            {
                Version? version = Assembly.GetExecutingAssembly().GetName().Version;
                string? targetFrameworkName = AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;
                DateTime creationTime = File.GetCreationTime(AppContext.BaseDirectory);
                OperatingSystem oSVersion = Environment.OSVersion;

                Console.WriteLine($@"
███    █▄  ███▄▄▄▄    ▄█   ▄████████    ▄████████ ▀█████████▄
███    ███ ███▀▀▀██▄ ███  ███    ███   ███    ███   ███    ███
███    ███ ███   ███ ███▌ ███    █▀    ███    ███   ███    ███
███    ███ ███   ███ ███▌ ███          ███    ███  ▄███▄▄▄██▀
███    ███ ███   ███ ███▌ ███        ▀███████████ ▀▀███▀▀▀██▄
███    ███ ███   ███ ███  ███    █▄    ███    ███   ███    ██▄ {version} - {targetFrameworkName}
███    ███ ███   ███ ███  ███    ███   ███    ███   ███    ███ {creationTime}
████████▀   ▀█   █▀  █▀   ████████▀    ███    █▀  ▄█████████▀ {oSVersion}
");
                Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
                IConfigurationService configurationService = new ConfigurationService();
                Configuration configuration = configurationService.FromArgs(args);
                IDdfFileService ddfFileService = new DdfFileService(configuration);
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
                Process process = new()
                {
                    StartInfo = startInfo
                };

                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.OutputDataReceived += Process_OutputDataReceived;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(Main));

                Environment.ExitCode = 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}