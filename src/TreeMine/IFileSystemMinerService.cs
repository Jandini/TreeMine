using System.Collections.Generic;
using System.IO;

namespace TreeMine
{
    internal interface IFileSystemMinerService
    {
        IEnumerable<FileSystemArtifact> MineFileSystem(DirectoryInfo root);
    }
}