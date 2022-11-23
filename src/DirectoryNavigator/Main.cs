using Microsoft.Extensions.Logging;
using System.IO;
using System;
using Serilog;
using System.Linq;
using System.Collections.Generic;

namespace DirectoryNavigator
{
    internal class Main : IMain
    {
        private readonly ILogger<Main> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDirectoryTreeNavigator _navigator;

        public Main(ILogger<Main> logger, ILoggerFactory loggerFactory, IDirectoryTreeNavigator navigator)
        {
            _logger = logger;
            _navigator = navigator;
            _loggerFactory = loggerFactory;
        }


        private static void WriteStats(DirectoryTreeStats stats, DirectoryTreeInfo info)
        {
            stats.Add(info.Item);

            if ((stats.FileCount % 1024) == 0)
                Console.Title = $"Found {stats.FileCount} files | {stats.DirCount} directories | {stats.TotalFileSize} bytes";
        }


        IDisposable CreateLogger(DirectoryInfo info) => _loggerFactory
            .AddSerilog(new LoggerConfiguration()
            .WriteTo.File(Path.ChangeExtension(Path.Combine(info.Parent.FullName, info.Name), "log"))
                .CreateLogger(), dispose: true);
    

        public void Count(string path)
        {
            var stats = new DirectoryTreeStats();
            var root = new DirectoryInfo(path);

            if (!root.Exists)
                throw new DirectoryNotFoundException();

            using (CreateLogger(root))
            {
                _logger.LogInformation("Counting {root}", root.FullName);

                foreach (var info in _navigator.NavigateDirectoryTree(root))
                    WriteStats(stats, info);

                _logger.LogInformation("Found {files} files | {dirs} directories | {bytes} bytes", stats.FileCount, stats.DirCount, stats.TotalFileSize);
            }
        }


        public void Scan(string path)
        {

            var stats = new DirectoryTreeStats();
            var root = new DirectoryInfo(path);

            if (!root.Exists)
                throw new DirectoryNotFoundException();

            using (CreateLogger(root))
            {
                _logger.LogInformation("Scanning {root}", root.FullName);
    
                foreach (var info in _navigator.NavigateDirectoryTree(root))
                {
                    WriteStats(stats, info);
                    _logger.LogInformation("{level} {@id}  {@parent} {item}", info.Level.ToString().PadLeft(2), info.Id, info.Parent, info.Item.FullName[(root.FullName.Length + 1)..]);
                }

                _logger.LogInformation("Found {files} files | {dirs} directories | {bytes} bytes", stats.FileCount, stats.DirCount, stats.TotalFileSize);
            }
        }

        public void Hash(string path)
        {
            var stats = new DirectoryTreeStats();
            var root = new DirectoryInfo(path);
            var dirs = new List<DirectoryTreeHashInfo>();

            if (!root.Exists)
                throw new DirectoryNotFoundException();

            using (CreateLogger(root))
            {
                _logger.LogInformation("Hashing {root}", root.FullName);

                foreach (var info in _navigator.HashDirectoryTree(root).OrderBy(a => a.Hash))
                {
                    dirs.Add(info);
                    WriteStats(stats, info);
                    _logger.LogInformation("{hash} {level} {item} ", info.Hash, info.Level.ToString().PadLeft(2), info.Item.FullName[(root.FullName.Length + 1)..]);
                }

                var unique = dirs.DistinctBy(a => a.Hash);
                _logger.LogInformation("Found {dirs} directories | {count} unique directores ", stats.DirCount, unique.Count());

                
            }
        }
    }
}