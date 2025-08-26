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
            CompressionWindowSize = args.Length > 5 && Enum.TryParse<CompressionWindowSize>(args[5], true, out var compressionWindowSize) ? compressionWindowSize : CompressionWindowSize.MB2,
            LogEnabled = args.Length > 6 && bool.TryParse(args[6], out var log) && log,
        };
    }
}