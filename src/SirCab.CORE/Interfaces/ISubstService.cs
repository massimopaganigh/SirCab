namespace SirCab.CORE.Interfaces
{
    /// <summary>
    /// Provides services for managing Windows SUBST drive mappings.
    /// </summary>
    public interface ISubstService
    {
        /// <summary>
        /// Creates a virtual drive mapping for the specified source directory.
        /// </summary>
        /// <param name="sourceDirectory">The source directory to map to a virtual drive.</param>
        /// <returns>The root path of the created virtual drive, or null if the operation failed.</returns>
        public string? Create(string sourceDirectory);

        /// <summary>
        /// Deletes the specified virtual drive mapping.
        /// </summary>
        /// <param name="driveLetter">The drive letter to delete the mapping for.</param>
        public void Delete(string driveLetter);
    }
}