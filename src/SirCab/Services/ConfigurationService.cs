namespace SirCab.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public Configuration FromArgs(string[] args) => new()
        {
            SourceDirectory = args[0],
            DestinationDirectory = args[1],
            FileName = args[2]
        };
    }
}