using System.Collections.Generic;
using System.IO;

namespace TreeMine.Services
{
    internal interface IDirectoryMinerService
    {
        IEnumerable<FileSystemArtifactEx> MineDirectories(DirectoryInfo root);
    }
}