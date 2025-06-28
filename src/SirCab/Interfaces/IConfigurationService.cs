namespace SirCab.Interfaces
{
    public interface IConfigurationService
    {
        public Configuration FromArgs(string[] args);
    }
}