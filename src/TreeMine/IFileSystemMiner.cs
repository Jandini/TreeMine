using System.Collections.Generic;
using System.IO;

namespace TreeMine
{
    internal interface IFileSystemMiner
    {
        IEnumerable<DirectoryTreeInfo> MineFileSystem(DirectoryInfo root);
    }
}