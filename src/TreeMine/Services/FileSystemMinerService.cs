using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace TreeMine.Services
{
    internal class FileSystemMinerService : IFileSystemMinerService
    {

        private readonly ILogger<DirectoryMinerService> _logger;

        public FileSystemMinerService(ILogger<DirectoryMinerService> logger)
        {
            _logger = logger;
        }


        public IEnumerable<FileSystemArtifact> MineFileSystem(DirectoryInfo root) => FileSystemMiner.Mine(
            new FileSystemArtifact()
            {
                Item = root,
                Id = Guid.Empty,
                Parent = Guid.Empty
            }, null, null, (ex) => { _logger.LogError(ex, ex.Message); return true; });
    }
}
