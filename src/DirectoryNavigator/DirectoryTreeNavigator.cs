using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DirectoryNavigator
{
    internal class DirectoryTreeNavigator : IDirectoryTreeNavigator
    {

        private readonly ILogger<DirectoryTreeNavigator> _logger;

        public DirectoryTreeNavigator(ILogger<DirectoryTreeNavigator> logger)
        {
            _logger = logger;
        }

        public IEnumerable<DirectoryTreeInfo> NavigateDirectoryTree(DirectoryInfo root) => NavigateDirectoryTree(new DirectoryTreeInfo() { Item = root, Id = Guid.Empty, Parent = Guid.Empty });

        public IEnumerable<DirectoryTreeInfo> NavigateDirectoryTree(DirectoryTreeInfo treeInfo)
        {
            IEnumerable<FileSystemInfo> dirContent = null;

            try
            {
                dirContent = (treeInfo.Item as DirectoryInfo).GetFileSystemInfos();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            var thisDir = new DirectoryTreeInfo() { Id = treeInfo.Id, Parent = treeInfo.Parent, Level = treeInfo.Level, Item = treeInfo.Item };

            // Root folder will not be returned
            if (treeInfo.Id != Guid.Empty)
                yield return thisDir;

            // Check if the content was retrivied successfully
            if (dirContent != null)
            {
                // Return directories recursively
                foreach (DirectoryInfo dirInfo in dirContent.OfType<DirectoryInfo>())
                    foreach (var subDirInfo in NavigateDirectoryTree(new DirectoryTreeInfo() { Id = Guid.NewGuid(), Level = thisDir.Level + 1, Parent = thisDir.Id, Item = dirInfo }))
                        yield return subDirInfo;

                // Return files
                foreach (FileInfo fi in dirContent.OfType<FileInfo>())
                    yield return new DirectoryTreeInfo() { Id = Guid.NewGuid(), Parent = thisDir.Id, Level = thisDir.Level, Item = fi };
            }
        }
    }
}
