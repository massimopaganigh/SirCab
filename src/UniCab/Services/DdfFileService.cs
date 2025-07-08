namespace UniCab.Services
{
    public class DdfFileService(Configuration configuration) : IDdfFileService
    {
        private const int _compressionMemory = 21;

        private readonly string? _sourceDirectory = configuration.SourceDirectory;
        private readonly string? _destinationDirectory = configuration.DestinationDirectory;
        private readonly string? _fileName = configuration.FileName;
        private readonly CompressionType? _compressionType = configuration.CompressionType;

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
                    || string.IsNullOrEmpty(_fileName)
                    || _compressionType == null)
                {
                    Log.Error("Source directory, destination directory, file name or compression type is null or empty.");
                    Log.Warning("Usage: UniCab.exe <sourceDirectory> <destinationDirectory> <fileName> <compressionType>");

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
.Set Cabinet=on");

                switch (_compressionType)
                {
                    case CompressionType.None:
                        ddfFileContent.AppendLine(".Set Compress=off");
                        break;
                    case CompressionType.MSZIP:
                        ddfFileContent.AppendLine(@".Set Compress=on
.Set CompressionType=MSZIP");
                        break;
                    case CompressionType.LZX:
                        ddfFileContent.AppendLine($@".Set Compress=on
.Set CompressionType=LZX
.Set CompressionMemory={_compressionMemory}");
                        break;
                }

                int ddfFileRowInt = ddfFileContent.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length;
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