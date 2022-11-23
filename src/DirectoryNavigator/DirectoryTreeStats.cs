using System.IO;

namespace DirectoryNavigator
{
    public sealed class DirectoryTreeStats
    {
        public int DirCount { get; set; }
        public int FileCount { get; set; }
        public long TotalFileSize { get; set; }


        public void Add(FileSystemInfo info)
        {
            if (info is FileInfo file)
            {
                FileCount++;
                TotalFileSize += file.Length;
            }
            else
            {
                DirCount++;
            }            
        }
    }
}
