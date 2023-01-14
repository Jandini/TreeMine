using System.Collections.Generic;
using System.IO;

namespace TreeMine
{
    internal interface IDirectoryMinerService
    {
        IEnumerable<FileSystemArtifactEx> MineDirectories(DirectoryInfo root);
    }
}