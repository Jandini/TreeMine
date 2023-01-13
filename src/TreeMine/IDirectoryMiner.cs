using System.Collections.Generic;
using System.IO;

namespace TreeMine
{
    internal interface IDirectoryMiner
    {
        IEnumerable<DirectoryTreeHashInfo> MineDirectories(DirectoryInfo root);
    }
}