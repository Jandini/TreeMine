using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TreeMine
{
    public static class TreeMiner
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dirArtifact"></param>
        /// <param name="onDirArtifact"></param>
        /// <param name="onFileArtifact"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        public static IEnumerable<T> Mine<T>(T dirArtifact, Func<T, IEnumerable<FileSystemInfo>, bool> onDirArtifact, Func<T, bool> onFileArtifact, Action<Exception> onException) where T : IFileSystemArtifact, new()
        {
            IEnumerable<FileSystemInfo> dirContent = null;

            try
            {
                // Get directory content, both files and directories
                if (dirArtifact.Item is DirectoryInfo dirInfo)
                    dirContent = dirInfo.GetFileSystemInfos();
            }
            catch (Exception ex)
            {
                if (onException != null)
                    onException(ex);
                else
                    throw;
            }
          

            // Check if the content was retrivied successfully. The dirContent can be null if exception handler call back is provided.
            if (dirContent != null)
            {
                // Create directory tree info for this folder
                var thisDir = new T() { Id = dirArtifact.Id, Parent = dirArtifact.Parent, Level = dirArtifact.Level, Item = dirArtifact.Item };

                if ((onDirArtifact?.Invoke(thisDir, dirContent) ?? true) && dirArtifact.Id != Guid.Empty)
                    // Return this folder only if that is not root folder 
                    yield return thisDir;
                          
                // Return directories recursively
                foreach (DirectoryInfo dirInfo in dirContent.OfType<DirectoryInfo>())                
                    foreach (var subDirInfo in Mine(new T() { Id = Guid.NewGuid(), Level = thisDir.Level + 1, Parent = thisDir.Id, Item = dirInfo }, onDirArtifact, onFileArtifact, onException))
                        yield return subDirInfo;
                
                if (onFileArtifact != null)
                    // Create and return file artifact found in directory content
                    foreach (FileInfo fi in dirContent.OfType<FileInfo>())
                    {
                        var fileArtifact = new T() { Id = Guid.NewGuid(), Parent = thisDir.Id, Level = thisDir.Level, Item = fi };
                        
                        if (onFileArtifact.Invoke(fileArtifact))
                            yield return fileArtifact;
                    }
            }
        }
    }
}
