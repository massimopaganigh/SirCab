namespace SirCab.Interfaces
{
    /// <summary>
    /// Provides services for configuration management and parsing.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Parses command line arguments into a configuration object.
        /// </summary>
        /// <param name="args">The command line arguments to parse.</param>
        /// <returns>A <see cref="Configuration"/> object containing the parsed configuration values.</returns>
        public Configuration FromArgs(string[] args);
    }
}