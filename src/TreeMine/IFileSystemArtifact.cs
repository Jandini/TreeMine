using System;
using System.IO;

namespace TreeMine
{
    public interface IFileSystemArtifact
    {
        Guid Id { get; set; }
        Guid Parent { get; set; }
        int Level { get; set; }
        FileSystemInfo Info { get; set; }
    }
}
