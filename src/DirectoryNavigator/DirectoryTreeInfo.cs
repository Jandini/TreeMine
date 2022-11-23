using System;
using System.IO;

namespace DirectoryNavigator
{
    public class DirectoryTreeInfo
    {
        public Guid Id { get; set; }
        public Guid Parent { get; set; }
        public int Level { get; set; }
        public FileSystemInfo Item { get; set; }
    }
}
