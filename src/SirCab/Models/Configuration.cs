﻿namespace SirCab.Models
{
    public class Configuration
    {
        public string? SourceDirectory { get; set; }

        public string? DestinationDirectory { get; set; }

        public string? FileName { get; set; }

        public CompressionType? CompressionType { get; set; }
    }
}