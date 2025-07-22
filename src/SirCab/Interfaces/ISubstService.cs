namespace SirCab.Interfaces
{
    public interface ISubstService
    {
        public string? Create(string sourceDirectory);

        public void Delete(string driveLetter);
    }
}