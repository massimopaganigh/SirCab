namespace SirCab.CORE.Extensions
{
    public static class StringExtension
    {
        public static string? FromRootToString(this string? input)
        {
            string? output = input;

            if (input != null
                && input.EndsWith(":\\"))
                output = input.TrimEnd(':', '\\');

            return output;
        }

        public static string? FromStringToRoot(this string? input)
        {
            string? output = input;

            if (input != null
                && !input.EndsWith('\\'))
                output = string.Format("{0}:\\", input);

            return output;
        }

        public static string? WithQuotes(this string? input)
        {
            string? output = input;

            if (input != null
                && !(input.StartsWith('\"')
                && input.EndsWith('\"')))
                output = string.Format("\"{0}\"", input);

            return output;
        }
    }
}