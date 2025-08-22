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

        /// <summary>
        /// Downloads and installs the latest version of the application.
        /// </summary>
        /// <returns>A task that represents the asynchronous download and install operation.</returns>
        public Task DownloadAndInstallUpdateAsync();

        /// <summary>
        /// Retrieves the current version of the application from the remote repository.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current version string, or null if the version could not be retrieved.</returns>
        public Task<string?> GetCurrentVersionAsync();
    }
}