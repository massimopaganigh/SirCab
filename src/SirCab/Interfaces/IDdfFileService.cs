namespace SirCab.Interfaces
{
    /// <summary>
    /// Provides services for DDF (Data Definition File) operations.
    /// </summary>
    public interface IDdfFileService
    {
        /// <summary>
        /// Creates a DDF file based on the current configuration.
        /// </summary>
        /// <returns>The path of the created DDF file, or null if the operation failed.</returns>
        public string? Create();
    }
}