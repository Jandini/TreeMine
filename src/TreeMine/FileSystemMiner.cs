using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace TreeMine
{
    internal class FileSystemMiner : IFileSystemMiner
    {

        private readonly ILogger<DirectoryMiner> _logger;

        public FileSystemMiner(ILogger<DirectoryMiner> logger)
        {
            _logger = logger;
        }
      

        public IEnumerable<DirectoryTreeInfo> MineFileSystem(DirectoryInfo root) 
            => MineFileSystem(new DirectoryTreeInfo() { 
                Item = root, 
                Id = Guid.Empty, 
                Parent = Guid.Empty 
            });

        public IEnumerable<DirectoryTreeInfo> MineFileSystem(DirectoryTreeInfo treeInfo)
        {
            IEnumerable<FileSystemInfo> dirContent = null;

            try
            {
                // Get directory content, both files and directories
                dirContent = (treeInfo.Item as DirectoryInfo)?.GetFileSystemInfos();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            // Create directory tree info for this folder
            var thisDir = new DirectoryTreeInfo() { Id = treeInfo.Id, Parent = treeInfo.Parent, Level = treeInfo.Level, Item = treeInfo.Item };

            if (treeInfo.Id != Guid.Empty)
                // Return this folder only if that is not root folder 
                yield return thisDir;

            // Check if the content was retrivied successfully
            if (dirContent != null)
            {
                // Return directories recursively
                foreach (DirectoryInfo dirInfo in dirContent.OfType<DirectoryInfo>())
                    foreach (var subDirInfo in MineFileSystem(new DirectoryTreeInfo() { Id = Guid.NewGuid(), Level = thisDir.Level + 1, Parent = thisDir.Id, Item = dirInfo }))
                        yield return subDirInfo;

                // Return files
                foreach (FileInfo fi in dirContent.OfType<FileInfo>())
                    yield return new DirectoryTreeInfo() { Id = Guid.NewGuid(), Parent = thisDir.Id, Level = thisDir.Level, Item = fi };
            }
        }    
    }
}
