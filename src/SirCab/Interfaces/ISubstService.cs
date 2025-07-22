namespace SirCab.Interfaces
{
    /// <summary>
    /// Provides services for managing virtual drives using the SUBST command.
    /// </summary>
    public interface ISubstService
    {
        /// <summary>
        /// Creates a virtual drive for the specified source directory.
        /// </summary>
        /// <param name="sourceDirectory">The source directory to associate with the virtual drive.</param>
        /// <returns>The drive letter of the created virtual drive, or null if the operation failed.</returns>
        public string? Create(string sourceDirectory);

        /// <summary>
        /// Deletes the virtual drive associated with the specified drive letter.
        /// </summary>
        /// <param name="driveLetter">The drive letter of the virtual drive to delete.</param>
        public void Delete(string driveLetter);
    }
}