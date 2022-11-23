using System.Collections.Generic;
using System.IO;

namespace DirectoryNavigator
{
    internal interface IDirectoryTreeNavigator
    {
        IEnumerable<DirectoryTreeInfo> NavigateDirectoryTree(DirectoryInfo root);
    }
}