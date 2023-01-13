using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TreeMine
{
    internal class DirectoryMiner : IDirectoryMiner
    {
        private readonly ILogger<DirectoryMiner> _logger;

        public DirectoryMiner(ILogger<DirectoryMiner> logger)
        {
            _logger = logger;
        }
      
        public IEnumerable<DirectoryTreeHashInfo> MineDirectories(DirectoryInfo root) => 
            MineDirectories(new DirectoryTreeHashInfo() { 
                Item = root, 
                Id = Guid.Empty, 
                Parent = Guid.Empty, 
                Hash = Convert.ToHexString(MD5.Create().ComputeHash(Array.Empty<byte>()))
        });



        public IEnumerable<DirectoryTreeHashInfo> MineDirectories(DirectoryTreeHashInfo treeInfo)
        {
            IEnumerable<FileSystemInfo> dirContent = null;

            try
            {
                // Get directory content, both files and directories
                dirContent = (treeInfo.Item as DirectoryInfo)?.GetFileSystemInfos();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            // Check if the content was retrivied successfully
            if (dirContent != null)
            {
                // Create content string ordered by file name and size
                var content = string.Join(';', dirContent.OrderBy(a => a.Name).Select(s => s.Name + ((s as FileInfo)?.Length ?? 0).ToString()));
                var hash = Convert.ToHexString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(content)));

                var thisDir = new DirectoryTreeHashInfo() { Id = treeInfo.Id, Parent = treeInfo.Parent, Level = treeInfo.Level, Item = treeInfo.Item, Hash = hash };

                if (treeInfo.Id != Guid.Empty)
                    yield return thisDir;

                // Return directories recursively
                foreach (DirectoryInfo dirInfo in dirContent.OfType<DirectoryInfo>())
                    foreach (var subDirInfo in MineDirectories(new DirectoryTreeHashInfo() { Id = Guid.NewGuid(), Level = thisDir.Level + 1, Parent = thisDir.Id, Item = dirInfo }))
                        yield return subDirInfo;
            }
        }
    }
}
