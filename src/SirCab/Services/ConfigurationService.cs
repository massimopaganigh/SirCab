namespace SirCab.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public Configuration FromArgs(string[] args) => new()
        {
            SourceDirectory = args.Length > 0 ? args[0] : null,
            DestinationDirectory = args.Length > 1 ? args[1] : null,
            FileName = args.Length > 2 ? args[2] : null
        };
    }
}