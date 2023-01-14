using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TreeMine
{
    internal class DirectoryMinerService : IDirectoryMinerService
    {
        private readonly ILogger<DirectoryMinerService> _logger;

        public DirectoryMinerService(ILogger<DirectoryMinerService> logger)
        {
            _logger = logger;
        }


        public IEnumerable<FileSystemArtifactEx> MineDirectories(DirectoryInfo root) =>
          TreeMiner.Mine(
              new FileSystemArtifactEx()
              {
                  Item = root,
                  Id = Guid.Empty,
                  Parent = Guid.Empty,
                  Hash = Convert.ToHexString(MD5.Create().ComputeHash(Array.Empty<byte>()))
              }, 
              (dir, content) =>
              {
                  var list = string.Join(';', content.OrderBy(a => a.Name).Select(s => string.Join(',', s.Name, ((s as FileInfo)?.Length ?? 0))));
                  dir.Hash = Convert.ToHexString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(list)));

                  return true;
              },
              null,
              (ex) => _logger.LogError(ex, ex.Message));


        //public IEnumerable<FileSystemArtifactEx> MineDirectories(DirectoryInfo root) =>
        //    MineDirectories(new FileSystemArtifactEx()
        //    {
        //        Item = root,
        //        Id = Guid.Empty,
        //        Parent = Guid.Empty,
        //        Hash = Convert.ToHexString(MD5.Create().ComputeHash(Array.Empty<byte>()))
        //    });



        //public IEnumerable<FileSystemArtifactEx> MineDirectories(FileSystemArtifactEx treeInfo)
        //{
        //    IEnumerable<FileSystemInfo> dirContent = null;

        //    try
        //    {
        //        // Get directory content, both files and directories
        //        dirContent = (treeInfo.Item as DirectoryInfo)?.GetFileSystemInfos();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //    }

        //    // Check if the content was retrivied successfully
        //    if (dirContent != null)
        //    {
        //        // Create content string ordered by file name and size
        //        var content = string.Join(';', dirContent.OrderBy(a => a.Name).Select(s => s.Name + ((s as FileInfo)?.Length ?? 0).ToString()));
        //        var hash = Convert.ToHexString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(content)));

        //        var thisDir = new FileSystemArtifactEx() { Id = treeInfo.Id, Parent = treeInfo.Parent, Level = treeInfo.Level, Item = treeInfo.Item, Hash = hash };

        //        if (treeInfo.Id != Guid.Empty)
        //            yield return thisDir;

        //        // Return directories recursively
        //        foreach (DirectoryInfo dirInfo in dirContent.OfType<DirectoryInfo>())
        //            foreach (var subDirInfo in MineDirectories(new FileSystemArtifactEx() { Id = Guid.NewGuid(), Level = thisDir.Level + 1, Parent = thisDir.Id, Item = dirInfo }))
        //                yield return subDirInfo;
        //    }
        //}
    }
}
