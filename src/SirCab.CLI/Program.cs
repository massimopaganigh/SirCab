namespace SirCab.CLI
{
    internal class Program
    {
        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            string errorData = e.Data ?? string.Empty;

            Log.Error($"(makecab.exe) {errorData}");

            if (string.IsNullOrEmpty(errorData))
                return;

            Environment.ExitCode = 1;
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) => Log.Information($"(makecab.exe) {e.Data ?? string.Empty}");

        public static async Task Main(string[] args)
        {
            try
            {
                Version? version = Assembly.GetExecutingAssembly().GetName().Version;
                string? targetFrameworkName = AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;
                DateTime creationTime = File.GetCreationTime(AppContext.BaseDirectory);
                OperatingSystem oSVersion = Environment.OSVersion;

                Console.WriteLine($@"
   ▄████████  ▄█     ▄████████  ▄████████    ▄████████ ▀█████████▄
  ███    ███ ███    ███    ███ ███    ███   ███    ███   ███    ███
  ███    █▀  ███▌   ███    ███ ███    █▀    ███    ███   ███    ███
  ███        ███▌  ▄███▄▄▄▄██▀ ███          ███    ███  ▄███▄▄▄██▀
▀███████████ ███▌ ▀▀███▀▀▀▀▀   ███        ▀███████████ ▀▀███▀▀▀██▄
         ███ ███  ▀███████████ ███    █▄    ███    ███   ███    ██▄ {version} - {targetFrameworkName}
   ▄█    ███ ███    ███    ███ ███    ███   ███    ███   ███    ███ {creationTime}
 ▄████████▀  █▀     ███    ███ ████████▀    ███    █▀  ▄█████████▀ {oSVersion}
                    ███    ███ by SirHurt CSR Team
");
                Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
                IConfigurationService configurationService = new ConfigurationService();
                Configuration configuration = configurationService.FromArgs(args);

                if (configuration.LogEnabled == true)
                {
                    string logFilePath = Path.Combine(AppContext.BaseDirectory, "SirCab.log");

                    if (File.Exists(logFilePath))
                        File.Delete(logFilePath);

                    Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File(logFilePath).CreateLogger();
                }

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

                List<string> createdFiles =
                [
                    Path.Combine(AppContext.BaseDirectory, "setup.inf"),
                    Path.Combine(AppContext.BaseDirectory, "setup.rpt")
                ];

                foreach (var createdFile in createdFiles)
                    if (File.Exists(createdFile))
                        File.Delete(createdFile);

                if (configuration.SubstError == false)
                {
                    substService.Delete(configuration.SourceDirectory!);
                    substService.Delete(configuration.DestinationDirectory!);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(Main));

                Environment.ExitCode = 1;
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