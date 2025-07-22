namespace UniCab.Services
{
    public class SubstService : ISubstService
    {
        private static string? GetLastAvailableDriveLetter()
        {
            for (char driveLetter = 'Z'; driveLetter >= 'A'; driveLetter--)
                if (!Directory.Exists(driveLetter.ToString().FromStringToRoot()))
                    return driveLetter.ToString();

            return null;
        }

        public string? Create(string sourceDirectory)
        {
            try
            {
                string? root = Path.GetPathRoot(sourceDirectory);
                string? driveLetter = GetLastAvailableDriveLetter();

                if (string.IsNullOrEmpty(sourceDirectory)
                    || string.IsNullOrEmpty(root)
                    || string.IsNullOrEmpty(driveLetter))
                {
                    Log.Error("Root or driver letter is null or empty.");

                    return null;
                }

                ProcessStartInfo startInfo = new()
                {
                    CreateNoWindow = true,
                    FileName = "subst.exe",
                    Arguments = $"{driveLetter.WithQuotes()}: {sourceDirectory.WithQuotes()}",
                    //RedirectStandardOutput = true,
                    //RedirectStandardError = true,
                    UseShellExecute = false
                };
                Process process = new()
                {
                    StartInfo = startInfo
                };

                process.Start();
                process.WaitForExit();

                return driveLetter.FromStringToRoot();
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(Create));

                return null;
            }
        }

        public void Delete(string driveLetter)
        {
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    CreateNoWindow = true,
                    FileName = "subst.exe",
                    Arguments = $"{driveLetter.FromRootToString().WithQuotes()}: /d",
                    //RedirectStandardOutput = true,
                    //RedirectStandardError = true,
                    UseShellExecute = false
                };
                Process process = new()
                {
                    StartInfo = startInfo
                };

                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(Delete));

                throw;
            }
        }
    }
}