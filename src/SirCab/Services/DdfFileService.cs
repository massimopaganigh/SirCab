namespace SirCab.Services
{
    public class DdfFileService(Configuration configuration) : IDdfFileService
    {
        private const string _compressionType = "LZX";
        private const int _compressionMemory = 21;
        private const int _maxDdfFileRowInt = 1024;

        private readonly string? _sourceDirectory = configuration.SourceDirectory;
        private readonly string? _destinationDirectory = configuration.DestinationDirectory;
        private readonly string? _fileName = configuration.FileName;

        private static List<DdfFileRow> GetDdfFileRowList(string directory) => GetDdfFileRowList(directory, directory);

        private static List<DdfFileRow> GetDdfFileRowList(string directory, string rootDirectory)
        {
            List<DdfFileRow> ddfFileRowList = [];

            foreach (string file in Directory.GetFiles(directory))
            {
                ddfFileRowList.Add(new DdfFileRow
                {
                    FullName = file,
                    Path = Path.GetRelativePath(rootDirectory, file)
                });

                Log.Information("{0} processed.", file);
            }

            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                ddfFileRowList.AddRange(GetDdfFileRowList(subDirectory, rootDirectory));

                Log.Information("{0} processed.", subDirectory);
            }

            return ddfFileRowList;
        }

        public string? Create()
        {
            try
            {
                if (string.IsNullOrEmpty(_sourceDirectory)
                    || string.IsNullOrEmpty(_destinationDirectory)
                    || string.IsNullOrEmpty(_fileName))
                {
                    Log.Error("Source, destination, or file name is null or empty.");

                    return null;
                }

                if (!Directory.Exists(_sourceDirectory))
                {
                    Log.Error("Source does not exist.");

                    return null;
                }

                if (!Directory.Exists(_destinationDirectory))
                {
                    Directory.CreateDirectory(_destinationDirectory);

                    Log.Information("{0} created.", _destinationDirectory);
                }

                StringBuilder ddfFileContent = new();

                ddfFileContent.AppendLine($@";*** MakeCAB Directive file;
.OPTION EXPLICIT
.Set CabinetNameTemplate={$"{_fileName}.cab".WithQuotes()}
.Set DiskDirectory1={_destinationDirectory.WithQuotes()}
.Set MaxDiskSize=0
.Set Cabinet=on
.Set Compress=on
.Set CompressionType={_compressionType}
.Set CompressionMemory={_compressionMemory}");

                int ddfFileRowInt = ddfFileContent.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length;

                ddfFileRowInt = _maxDdfFileRowInt - ddfFileRowInt;

                List<DdfFileRow> ddfFileRowList = GetDdfFileRowList(_sourceDirectory);

                foreach (DdfFileRow ddfFileRow in ddfFileRowList.Take(ddfFileRowInt))
                    ddfFileContent.AppendLine(ddfFileRow.Row);

                string ddfFileName = $"{_fileName}.ddf";
                string ddfFilePath = Path.Combine(_destinationDirectory, ddfFileName);

                File.WriteAllText(ddfFilePath, ddfFileContent.ToString(), Encoding.Default);
                Log.Information("{0} created.", ddfFileName);

                return ddfFilePath;
            }
            catch (Exception ex)
            {
                Log.Error(ex, nameof(Create));

                return null;
            }
        }
    }
}