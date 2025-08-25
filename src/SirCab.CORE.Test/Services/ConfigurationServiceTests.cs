namespace SirCab.CORE.Test.Services
{
    public class ConfigurationServiceTests
    {
        private readonly ConfigurationService _configurationService;

        public ConfigurationServiceTests() => _configurationService = new ConfigurationService();

        #region FromArgs Tests

        [Fact]
        public void FromArgs_WithEmptyArray_ReturnsConfigurationWithAllNullProperties()
        {
            // Arrange
            string[] args = [];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.SourceDirectory);
            Assert.Null(result.DestinationDirectory);
            Assert.Null(result.FileName);
            Assert.Null(result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithOneArgument_SetsSourceDirectoryOnly()
        {
            // Arrange
            string[] args = ["C:\\Source"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("C:\\Source", result.SourceDirectory);
            Assert.Null(result.DestinationDirectory);
            Assert.Null(result.FileName);
            Assert.Null(result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithTwoArguments_SetsSourceAndDestinationDirectories()
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("C:\\Source", result.SourceDirectory);
            Assert.Equal("C:\\Destination", result.DestinationDirectory);
            Assert.Null(result.FileName);
            Assert.Null(result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithThreeArguments_SetsSourceDestinationAndFileName()
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("C:\\Source", result.SourceDirectory);
            Assert.Equal("C:\\Destination", result.DestinationDirectory);
            Assert.Equal("test.cab", result.FileName);
            Assert.Null(result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithFiveArguments_ValidCompressionType_SetsAllProperties()
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", "Cab", "MSZIP"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("C:\\Source", result.SourceDirectory);
            Assert.Equal("C:\\Destination", result.DestinationDirectory);
            Assert.Equal("test.cab", result.FileName);
            Assert.Equal(FileType.Cab, result.FileType);
            Assert.Equal(CompressionType.MSZIP, result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithValidCompressionType_None_SetsCompressionTypeToNone()
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", "Cab", "None"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CompressionType.None, result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithValidCompressionType_LZX_SetsCompressionTypeToLZX()
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", "Cab", "LZX"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CompressionType.LZX, result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithCompressionType_CaseInsensitive_SetsCorrectCompressionType()
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", "Cab", "mszip"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CompressionType.MSZIP, result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithCompressionType_MixedCase_SetsCorrectCompressionType()
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", "Cab", "LzX"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CompressionType.LZX, result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithInvalidCompressionType_SetsCompressionTypeToNull()
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", "InvalidType"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("C:\\Source", result.SourceDirectory);
            Assert.Equal("C:\\Destination", result.DestinationDirectory);
            Assert.Equal("test.cab", result.FileName);
            Assert.Null(result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithEmptyStringCompressionType_SetsCompressionTypeToNull()
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", ""];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithMoreThanFiveArguments_IgnoresExtraArguments()
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", "Cab", "MSZIP", "ExtraArg1", "ExtraArg2"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("C:\\Source", result.SourceDirectory);
            Assert.Equal("C:\\Destination", result.DestinationDirectory);
            Assert.Equal("test.cab", result.FileName);
            Assert.Equal(CompressionType.MSZIP, result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithEmptyStringArguments_SetsEmptyStrings()
        {
            // Arrange
            string[] args = ["", "", "", "", ""];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("", result.SourceDirectory);
            Assert.Equal("", result.DestinationDirectory);
            Assert.Equal("", result.FileName);
            Assert.Null(result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithNullArgumentInArray_SetsNullValue()
        {
            // Arrange
            string[] args = [null!, "C:\\Destination", "test.cab", "Cab", "MSZIP"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.SourceDirectory);
            Assert.Equal("C:\\Destination", result.DestinationDirectory);
            Assert.Equal("test.cab", result.FileName);
            Assert.Equal(CompressionType.MSZIP, result.CompressionType);
        }

        [Fact]
        public void FromArgs_WithWhitespaceArguments_PreservesWhitespace()
        {
            // Arrange
            string[] args = [" ", "  ", "   ", "Cab", "MSZIP"];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(" ", result.SourceDirectory);
            Assert.Equal("  ", result.DestinationDirectory);
            Assert.Equal("   ", result.FileName);
            Assert.Equal(CompressionType.MSZIP, result.CompressionType);
        }

        [Theory]
        [InlineData("None", CompressionType.None)]
        [InlineData("MSZIP", CompressionType.MSZIP)]
        [InlineData("LZX", CompressionType.LZX)]
        [InlineData("none", CompressionType.None)]
        [InlineData("mszip", CompressionType.MSZIP)]
        [InlineData("lzx", CompressionType.LZX)]
        [InlineData("NONE", CompressionType.None)]
        [InlineData("MsZiP", CompressionType.MSZIP)]
        [InlineData("LzX", CompressionType.LZX)]
        public void FromArgs_WithValidCompressionTypes_ParsesCorrectly(string compressionTypeString, CompressionType expectedCompressionType)
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", "Cab", compressionTypeString];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCompressionType, result.CompressionType);
        }

        [Theory]
        [InlineData("0", CompressionType.None)]
        [InlineData("1", CompressionType.MSZIP)]
        [InlineData("2", CompressionType.LZX)]
        public void FromArgs_WithNumericCompressionTypes_ParsesCorrectly(string numericCompressionType, CompressionType expectedCompressionType)
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", "Cab", numericCompressionType];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCompressionType, result.CompressionType);
        }

        [Theory]
        [InlineData("InvalidType")]
        [InlineData("ZIP")]
        [InlineData("GZIP")]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("random")]
        [InlineData("NotANumber")]
        [InlineData("100.5")]
        public void FromArgs_WithInvalidCompressionTypes_SetsCompressionTypeToNull(string invalidCompressionType)
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", invalidCompressionType];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.CompressionType);
        }

        [Theory]
        [InlineData("-1")]
        [InlineData("3")]
        [InlineData("999")]
        public void FromArgs_WithOutOfRangeNumericCompressionTypes_ParsesAsUnknownEnumValue(string numericCompressionType)
        {
            // Arrange
            string[] args = ["C:\\Source", "C:\\Destination", "test.cab", "Cab", numericCompressionType];

            // Act
            var result = _configurationService.FromArgs(args);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.CompressionType);
        }

        #endregion
    }
}