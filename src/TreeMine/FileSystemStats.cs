using System.IO;

namespace TreeMine
{
    public sealed class FileSystemStats
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
