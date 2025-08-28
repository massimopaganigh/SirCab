namespace SirCab.CORE.Models
{
    public class Configuration
    {
        public string? SourceDirectory { get; set; }

        public string? DestinationDirectory { get; set; }

        public string? FileName { get; set; }

        public FileType? FileType { get; set; }

        public CompressionType? CompressionType { get; set; }

        public CompressionWindowSize? CompressionWindowSize { get; set; }

        public bool? LogEnabled { get; set; }

        public bool? SubstError { get; set; } = false;
    }
}