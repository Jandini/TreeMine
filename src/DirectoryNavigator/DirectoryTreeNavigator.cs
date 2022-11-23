using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Cryptography;
using System.Reflection;
using System.Text;

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
                    foreach (var subDirInfo in NavigateDirectoryTree(new DirectoryTreeInfo() { Id = Guid.NewGuid(), Level = thisDir.Level + 1, Parent = thisDir.Id, Item = dirInfo }))
                        yield return subDirInfo;

                // Return files
                foreach (FileInfo fi in dirContent.OfType<FileInfo>())
                    yield return new DirectoryTreeInfo() { Id = Guid.NewGuid(), Parent = thisDir.Id, Level = thisDir.Level, Item = fi };
            }
        }



        public IEnumerable<DirectoryTreeHashInfo> HashDirectoryTree(DirectoryInfo root) => 
            HashDirectoryTree(new DirectoryTreeHashInfo() { 
                Item = root, 
                Id = Guid.Empty, 
                Parent = Guid.Empty, 
                Hash = Convert.ToHexString(MD5.Create().ComputeHash(Array.Empty<byte>()))
        });



        public IEnumerable<DirectoryTreeHashInfo> HashDirectoryTree(DirectoryTreeHashInfo treeInfo)
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

            // Create content string ordered by file name and size
            var content = string.Join(';', dirContent.OrderBy(a => a.Name).Select(s => s.Name + ((s as FileInfo)?.Length ?? 0).ToString()));
            var hash = Convert.ToHexString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(content)));

            var thisDir = new DirectoryTreeHashInfo() { Id = treeInfo.Id, Parent = treeInfo.Parent, Level = treeInfo.Level, Item = treeInfo.Item, Hash = hash };

            if (treeInfo.Id != Guid.Empty)
                yield return thisDir;

            // Check if the content was retrivied successfully
            if (dirContent != null)
            {
                // Return directories recursively
                foreach (DirectoryInfo dirInfo in dirContent.OfType<DirectoryInfo>())
                    foreach (var subDirInfo in HashDirectoryTree(new DirectoryTreeHashInfo() { Id = Guid.NewGuid(), Level = thisDir.Level + 1, Parent = thisDir.Id, Item = dirInfo }))
                        yield return subDirInfo;
            }
        }

    }
}
