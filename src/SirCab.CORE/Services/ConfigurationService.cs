namespace SirCab.CORE.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public Configuration FromArgs(string[] args) => new()
        {
            SourceDirectory = args.Length > 0 ? args[0] : null,
            DestinationDirectory = args.Length > 1 ? args[1] : null,
            FileName = args.Length > 2 ? args[2] : null,
            FileType = args.Length > 3 && Enum.TryParse<FileType>(args[3], true, out var fileType) ? fileType : null,
            CompressionType = args.Length > 4 && Enum.TryParse<CompressionType>(args[4], true, out var compressionType) ? compressionType : null,
            LogEnabled = args.Length > 5 && bool.TryParse(args[5], out var log) && log,
        };
    }
}