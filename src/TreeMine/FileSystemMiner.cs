using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TreeMine
{
    public static class FileSystemMiner
    {
        public static IEnumerable<T> Mine<T>(DirectoryInfo root, Func<T, IEnumerable<FileSystemInfo>, bool> onDirArtifact, Func<T, bool> onFileArtifact, Func<Exception, bool> onException)
            where T : IFileSystemArtifact, new() => Mine(new T() { Info = root, Id = Guid.Empty, Parent = Guid.Empty }, onDirArtifact, onFileArtifact, onException);

        public static IEnumerable<T> Mine<T>(string root, Func<T, IEnumerable<FileSystemInfo>, bool> onDirArtifact, Func<T, bool> onFileArtifact, Func<Exception, bool> onException)
            where T : IFileSystemArtifact, new() => Mine(new T() { Info = new DirectoryInfo(root), Id = Guid.Empty, Parent = Guid.Empty }, onDirArtifact, onFileArtifact, onException);

        public static IEnumerable<IFileSystemArtifact> Mine(DirectoryInfo root, Func<IFileSystemArtifact, IEnumerable<FileSystemInfo>, bool> onDirArtifact, Func<IFileSystemArtifact, bool> onFileArtifact, Func<Exception, bool> onException)
            => Mine<FileSystemArtifact>(new FileSystemArtifact() { Info = root, Id = Guid.Empty, Parent = Guid.Empty }, onDirArtifact, onFileArtifact, onException);

        public static IEnumerable<IFileSystemArtifact> Mine(string root, Func<IFileSystemArtifact, IEnumerable<FileSystemInfo>, bool> onDirArtifact, Func<IFileSystemArtifact, bool> onFileArtifact, Func<Exception, bool> onException)
            => Mine<FileSystemArtifact>(new FileSystemArtifact() { Info = new DirectoryInfo(root), Id = Guid.Empty, Parent = Guid.Empty }, onDirArtifact, onFileArtifact, onException);

        public static IEnumerable<IFileSystemArtifact> Mine(string root)
          => Mine(new FileSystemArtifact() { Info = new DirectoryInfo(root), Id = Guid.Empty, Parent = Guid.Empty }, (dir, content) => true, (file) => true, (exception) => false);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">IFileSystemArtifact</typeparam>
        /// <param name="dirArtifact">Root directory artifact.</param>
        /// <param name="onDirArtifact">Enrich and filter directory artifacts. If false is returned then artifact will not be returned. </param>
        /// <param name="onFileArtifact">Enrich and filter file artifacts. </param>
        /// <param name="onException">Exception handler. If not provided or false is returned then exception is throw and mining is interrupted.</param>
        /// <returns>Directory and file artifacts.</returns>
        public static IEnumerable<T> Mine<T>(T dirArtifact, Func<T, IEnumerable<FileSystemInfo>, bool> onDirArtifact, Func<T, bool> onFileArtifact, Func<Exception, bool> onException) where T : IFileSystemArtifact, new()
        {
            IEnumerable<FileSystemInfo> dirContent = null;

            try
            {
                // Get directory content, both files and directories
                if (dirArtifact.Info is DirectoryInfo dirInfo)
                    dirContent = dirInfo.GetFileSystemInfos();
            }
            catch (Exception ex)
            {
                if (!(onException?.Invoke(ex) ?? false))
                    throw;
            }

            // Check if the content was retrivied successfully. The dirContent can be null if exception handler call back is provided.
            if (dirContent != null)
            {
                if ((onDirArtifact?.Invoke(dirArtifact, dirContent) ?? true) && dirArtifact.Id != Guid.Empty)
                    // Return this folder only if that is not root folder 
                    yield return dirArtifact;

                // Mine directories recursively
                foreach (DirectoryInfo dirInfo in dirContent.OfType<DirectoryInfo>())
                    foreach (var subDirInfo in Mine(new T() { Id = Guid.NewGuid(), Level = dirArtifact.Level + 1, Parent = dirArtifact.Id, Info = dirInfo }, onDirArtifact, onFileArtifact, onException))
                        yield return subDirInfo;

                if (onFileArtifact != null)
                {
                    // Create and return file artifacts found in directory content
                    foreach (FileInfo fileInfo in dirContent.OfType<FileInfo>())
                    {
                        var fileArtifact = new T() { Id = Guid.NewGuid(), Parent = dirArtifact.Id, Level = dirArtifact.Level, Info = fileInfo };
                        if (onFileArtifact.Invoke(fileArtifact))
                            yield return fileArtifact;
                    }
                }
            }
        }
    }
}
