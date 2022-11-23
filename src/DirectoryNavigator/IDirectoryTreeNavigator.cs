using System.Collections.Generic;
using System.IO;

namespace DirectoryNavigator
{
    internal interface IDirectoryTreeNavigator
    {
        IEnumerable<DirectoryTreeInfo> NavigateDirectoryTree(DirectoryInfo root);

        IEnumerable<DirectoryTreeHashInfo> HashDirectoryTree(DirectoryInfo root);
    }
}