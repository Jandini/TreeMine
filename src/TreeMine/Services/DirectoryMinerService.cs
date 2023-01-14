using System.Collections.Generic;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TreeMine.Services
{
    internal class DirectoryMinerService : IDirectoryMinerService
    {
        private readonly ILogger<DirectoryMinerService> _logger;

        public DirectoryMinerService(ILogger<DirectoryMinerService> logger)
        {
            _logger = logger;
        }


        public IEnumerable<FileSystemArtifactEx> MineDirectories(DirectoryInfo root) =>
          FileSystemMiner.Mine(
              new FileSystemArtifactEx()
              {
                  Info = root,
                  Id = Guid.Empty,
                  Parent = Guid.Empty,
                  Hash = Convert.ToHexString(MD5.Create().ComputeHash(Array.Empty<byte>()))
              },
              (dir, content) =>
              {
                  var list = string.Join(';', content.OrderBy(a => a.Name).Select(s => string.Join(',', s.Name, (s as FileInfo)?.Length ?? 0)));
                  dir.Hash = Convert.ToHexString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(list)));

                  return true;
              },
              null,
              (ex) => { _logger.LogError(ex, ex.Message); return true; });

    }
}
