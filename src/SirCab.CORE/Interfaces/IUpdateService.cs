namespace SirCab.CORE.Interfaces
{
    /// <summary>
    /// Provides services for checking application updates.
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// Checks for available application updates and logs if an update is available.
        /// </summary>
        /// <returns>A task that represents the asynchronous update check operation.</returns>
        public Task CheckForUpdateAsync();
    }
}