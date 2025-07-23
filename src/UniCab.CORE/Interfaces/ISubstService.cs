namespace UniCab.CORE.Interfaces
{
    public interface ISubstService
    {
        public string? Create(string sourceDirectory);

        public void Delete(string driveLetter);
    }
}