namespace SirCab.CORE.Services
{
    public class DdfFileService(Configuration configuration, ISubstService substService) : IDdfFileService
    {
        private const int _compressionMemory = 21;
        private const int _maxDdfFileRowInt = 4096;

        private /*readonly*/ string? _sourceDirectory = configuration.SourceDirectory;
        private /*readonly*/ string? _destinationDirectory = configuration.DestinationDirectory;
        private readonly string? _fileName = configuration.FileName;
        private readonly FileType? _fileType = configuration.FileType;
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

        private static string GetFileExtension(FileType fileType) => fileType switch
        {
            FileType.Cab => "cab",
            FileType.Wsp => "wsp",
            FileType.Xsn => "xsn",
            _ => "cab"
        };

        public string? Create()
        {
            try
            {
                if (string.IsNullOrEmpty(_sourceDirectory)
                    || string.IsNullOrEmpty(_destinationDirectory)
                    || string.IsNullOrEmpty(_fileName)
                    || _compressionType == null
                    || _fileType == null)
                {
                    Log.Error("Source directory, destination directory, file name, file type or compression type is null or empty.");
                    Log.Warning("Usage: SirCab.exe <sourceDirectory> <destinationDirectory> <fileName> <fileType> <compressionType> [<log>]");

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

                string? tmpSourceDirectory = substService.Create(_sourceDirectory);
                string? tmpDestinationDirectory = substService.Create(_destinationDirectory);

                if (string.IsNullOrEmpty(tmpSourceDirectory)
                    || string.IsNullOrEmpty(tmpDestinationDirectory)
                    || !Directory.Exists(tmpSourceDirectory)
                    || !Directory.Exists(tmpDestinationDirectory))
                {
                    configuration.SubstError = true;

                    Log.Error("Source directory or destination directory is null, empty or does not exist after subst. Falling back to original directories.");
                }
                else
                {
                    _sourceDirectory = tmpSourceDirectory;
                    _destinationDirectory = tmpDestinationDirectory;
                    configuration.SourceDirectory = _sourceDirectory;
                    configuration.DestinationDirectory = _destinationDirectory;
                }

                StringBuilder ddfFileContent = new();

                string fileExtension = GetFileExtension(_fileType.Value);

                ddfFileContent.AppendLine($@";*** MakeCAB Directive file;
.OPTION EXPLICIT
.Set CabinetNameTemplate={$"{_fileName}.{fileExtension}".WithQuotes()}
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