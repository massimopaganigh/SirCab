namespace SirCab.CORE.Test.Extensions
{
    public class StringExtensionTests
    {
        #region FromRootToString Tests

        [Fact]
        public void FromRootToString_WithNullInput_ReturnsNull()
        {
            // Arrange
            string? input = null;

            // Act
            var result = input.FromRootToString();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void FromRootToString_WithEmptyString_ReturnsEmptyString()
        {
            // Arrange
            string input = string.Empty;

            // Act
            var result = input.FromRootToString();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FromRootToString_WithRootPath_ReturnsStringWithoutRootSuffix()
        {
            // Arrange
            string input = "C:\\";

            // Act
            var result = input.FromRootToString();

            // Assert
            Assert.Equal("C", result);
        }

        [Fact]
        public void FromRootToString_WithDriveRootPath_ReturnsStringWithoutRootSuffix()
        {
            // Arrange
            string input = "D:\\";

            // Act
            var result = input.FromRootToString();

            // Assert
            Assert.Equal("D", result);
        }

        [Fact]
        public void FromRootToString_WithRegularPath_ReturnsUnchanged()
        {
            // Arrange
            string input = "C:\\Users\\Documents";

            // Act
            var result = input.FromRootToString();

            // Assert
            Assert.Equal("C:\\Users\\Documents", result);
        }

        [Fact]
        public void FromRootToString_WithPathNotEndingInRootFormat_ReturnsUnchanged()
        {
            // Arrange
            string input = "SomeString";

            // Act
            var result = input.FromRootToString();

            // Assert
            Assert.Equal("SomeString", result);
        }

        [Fact]
        public void FromRootToString_WithPathEndingInBackslashOnly_ReturnsUnchanged()
        {
            // Arrange
            string input = "C:\\Users\\";

            // Act
            var result = input.FromRootToString();

            // Assert
            Assert.Equal("C:\\Users\\", result);
        }

        [Fact]
        public void FromRootToString_WithPathEndingInColonOnly_ReturnsUnchanged()
        {
            // Arrange
            string input = "C:";

            // Act
            var result = input.FromRootToString();

            // Assert
            Assert.Equal("C:", result);
        }

        #endregion

        #region FromStringToRoot Tests

        [Fact]
        public void FromStringToRoot_WithNullInput_ReturnsNull()
        {
            // Arrange
            string? input = null;

            // Act
            var result = input.FromStringToRoot();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void FromStringToRoot_WithEmptyString_ReturnsRootFormat()
        {
            // Arrange
            string input = string.Empty;

            // Act
            var result = input.FromStringToRoot();

            // Assert
            Assert.Equal(":\\", result);
        }

        [Fact]
        public void FromStringToRoot_WithSingleDriveLetter_ReturnsRootFormat()
        {
            // Arrange
            string input = "C";

            // Act
            var result = input.FromStringToRoot();

            // Assert
            Assert.Equal("C:\\", result);
        }

        [Fact]
        public void FromStringToRoot_WithDriveLetter_ReturnsRootFormat()
        {
            // Arrange
            string input = "D";

            // Act
            var result = input.FromStringToRoot();

            // Assert
            Assert.Equal("D:\\", result);
        }

        [Fact]
        public void FromStringToRoot_WithStringAlreadyEndingInBackslash_ReturnsUnchanged()
        {
            // Arrange
            string input = "C:\\";

            // Act
            var result = input.FromStringToRoot();

            // Assert
            Assert.Equal("C:\\", result);
        }

        [Fact]
        public void FromStringToRoot_WithPathEndingInBackslash_ReturnsUnchanged()
        {
            // Arrange
            string input = "C:\\Users\\Documents\\";

            // Act
            var result = input.FromStringToRoot();

            // Assert
            Assert.Equal("C:\\Users\\Documents\\", result);
        }

        [Fact]
        public void FromStringToRoot_WithRegularString_ReturnsRootFormat()
        {
            // Arrange
            string input = "SomeString";

            // Act
            var result = input.FromStringToRoot();

            // Assert
            Assert.Equal("SomeString:\\", result);
        }

        [Fact]
        public void FromStringToRoot_WithMultipleCharacters_ReturnsRootFormat()
        {
            // Arrange
            string input = "ABC";

            // Act
            var result = input.FromStringToRoot();

            // Assert
            Assert.Equal("ABC:\\", result);
        }

        #endregion

        #region WithQuotes Tests

        [Fact]
        public void WithQuotes_WithNullInput_ReturnsNull()
        {
            // Arrange
            string? input = null;

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void WithQuotes_WithEmptyString_ReturnsQuotedEmptyString()
        {
            // Arrange
            string input = string.Empty;

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Equal("\"\"", result);
        }

        [Fact]
        public void WithQuotes_WithUnquotedString_ReturnsQuotedString()
        {
            // Arrange
            string input = "Hello World";

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Equal("\"Hello World\"", result);
        }

        [Fact]
        public void WithQuotes_WithAlreadyQuotedString_ReturnsUnchanged()
        {
            // Arrange
            string input = "\"Already Quoted\"";

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Equal("\"Already Quoted\"", result);
        }

        [Fact]
        public void WithQuotes_WithStringStartingWithQuoteOnly_ReturnsQuoted()
        {
            // Arrange
            string input = "\"Hello World";

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Equal("\"\"Hello World\"", result);
        }

        [Fact]
        public void WithQuotes_WithStringEndingWithQuoteOnly_ReturnsQuoted()
        {
            // Arrange
            string input = "Hello World\"";

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Equal("\"Hello World\"\"", result);
        }

        //[Fact]
        //public void WithQuotes_WithSingleQuote_ReturnsQuoted()
        //{
        //    // Arrange
        //    string input = "\"";

        //    // Act
        //    var result = input.WithQuotes();

        //    // Assert
        //    Assert.Equal("\"\"\"", result);
        //}

        [Fact]
        public void WithQuotes_WithOnlyQuotes_ReturnsUnchanged()
        {
            // Arrange
            string input = "\"\"";

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Equal("\"\"", result);
        }

        [Fact]
        public void WithQuotes_WithSingleCharacter_ReturnsQuoted()
        {
            // Arrange
            string input = "A";

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Equal("\"A\"", result);
        }

        [Fact]
        public void WithQuotes_WithWhitespace_ReturnsQuoted()
        {
            // Arrange
            string input = " ";

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Equal("\" \"", result);
        }

        [Fact]
        public void WithQuotes_WithFilePath_ReturnsQuoted()
        {
            // Arrange
            string input = "C:\\Program Files\\MyApp\\app.exe";

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Equal("\"C:\\Program Files\\MyApp\\app.exe\"", result);
        }

        [Fact]
        public void WithQuotes_WithAlreadyQuotedFilePath_ReturnsUnchanged()
        {
            // Arrange
            string input = "\"C:\\Program Files\\MyApp\\app.exe\"";

            // Act
            var result = input.WithQuotes();

            // Assert
            Assert.Equal("\"C:\\Program Files\\MyApp\\app.exe\"", result);
        }

        #endregion
    }
}