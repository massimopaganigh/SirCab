namespace SirCab.CORE.Services
{
    public class SubstService : ISubstService
    {
        private static string? GetLastAvailableDriveLetter()
        {
            HashSet<char> existingDrives = [.. DriveInfo.GetDrives().Select(d => d.Name[0])];

            for (char driveLetter = 'Z'; driveLetter >= 'A'; driveLetter--)
                if (!existingDrives.Contains(driveLetter))
                {
                    Log.Information($"Last available drive letter: {driveLetter}.");

                    return driveLetter.ToString();
                }

            Log.Error("No available drive letters found.");

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
                using Process process = new()
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
                using Process process = new()
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