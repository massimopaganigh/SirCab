namespace SirCab.Models
{
    public class DdfFileRow
    {
        public string? FullName { get; set; }

        public string? Path { get; set; }

        public string? Row => $"{FullName.WithQuotes} {Path.WithQuotes}";
    }
}