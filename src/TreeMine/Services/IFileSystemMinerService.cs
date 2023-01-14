using System.Collections.Generic;
using System.IO;

namespace TreeMine.Services
{
    internal interface IFileSystemMinerService
    {
        IEnumerable<FileSystemArtifact> MineFileSystem(DirectoryInfo root);
    }
}