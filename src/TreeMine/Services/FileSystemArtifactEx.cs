using System;
using System.IO;

namespace TreeMine.Services
{
    public class FileSystemArtifactEx : IFileSystemArtifact
    {
        public string Hash { get; set; }
        public Guid Id { get; set; }
        public Guid Parent { get; set; }
        public int Level { get; set; }
        public FileSystemInfo Item { get; set; }
    }
}
