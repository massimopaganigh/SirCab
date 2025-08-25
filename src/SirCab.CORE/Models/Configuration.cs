namespace SirCab.CORE.Models
{
    public class Configuration
    {
        public string? SourceDirectory { get; set; }

        public string? DestinationDirectory { get; set; }

        public string? FileName { get; set; }

        public FileType? FileType { get; set; }

        public CompressionType? CompressionType { get; set; }

        public bool? LogEnabled { get; set; } = false;

        public bool? SubstError { get; set; } = false;
    }
}