using Core.Utils;

namespace Core
{
    public class TreeGenerator
    {
        private int _currentId = 0;

        private GenFlags _flags = new();
        public Metadata metadata = new();
        public Statistics stats = new();

        public TreeGenerator(GenFlags genFlags)
        {
            _flags = genFlags;
            metadata.dirPath = _flags.targetDir;
            DirectoryInfo info = new(metadata.dirPath);
            metadata.dirName = info.Name;
            metadata.genDateTime = Calendar.GetDate().Replace("-", "/") + " " + Calendar.GetTime();
        }

        public IEnumerable<Node> GenerateTree()
        {
            _currentId = 0;
            try
            {
                // test if the root folder is accessible in the first place
                var test = Directory.EnumerateDirectories(_flags.targetDir);
                return TraverseDirectory(new DirectoryInfo(_flags.targetDir), null, 0);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CoreException(ErrorCode.Codes.RootAccessDenied, ex.Message);
                // if not, throw error and don't generate any report
                // why would someone need an 'empty' report?
            }
        }

        private IEnumerable<Node> TraverseDirectory(
            DirectoryInfo directory,
            int? parentId,
            int level
        )
        {
            int newId = _currentId++;

            // send current folder
            if (!_flags.filesOnly)
            {
                yield return new Node
                {
                    Id = newId,
                    ParentId = parentId,
                    Name = directory.Name,
                    Type = NodeType.Folder,
                    Level = level,
                };
            }
            if (level <= _flags.maxLevel)
            {
                // scan subfolders
                IEnumerable<DirectoryInfo>? subDirs = null;
                try
                {
                    subDirs = directory.EnumerateDirectories();
                }
                catch (UnauthorizedAccessException)
                {
                    // Ignore
                    if (!_flags.noStatistics)
                        stats.skippedFolders++;
                }
                if (subDirs != null)
                    foreach (var subDir in subDirs)
                    {
                        if (FileSystem.IsReparsePoint(subDir.FullName))
                        {
                            if (!_flags.noStatistics)
                                stats.files++;
                            yield return new Node
                            {
                                Id = _currentId++,
                                ParentId = newId,
                                Name = (_flags.noIcons ? null : "[→] ") + subDir.Name + " (link)",
                                Type = NodeType.File,
                                Level = level + 1,
                                Path = subDir.FullName,
                            };
                        }
                        else
                        {
                            if (!_flags.noStatistics)
                                stats.folders++;
                            foreach (var childNode in TraverseDirectory(subDir, newId, level + 1))
                            {
                                yield return childNode;
                            }
                        }
                    }

                // send files
                if (!_flags.dirsOnly)
                {
                    IEnumerable<string>? filePaths = null;
                    try
                    {
                        filePaths = Directory.EnumerateFiles(directory.FullName);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (!_flags.noStatistics)
                            stats.skippedFolders++;
                    }
                    if (filePaths != null)
                        foreach (var filePath in filePaths)
                        {
                            if (!_flags.noStatistics)
                            {
                                stats.files++;
                                FileInfo info = new(filePath);
                                if (info.Exists)
                                {
                                    stats.totalSizeBytes += info.Length;
                                    string ext = Path.GetExtension(filePath).ToLowerInvariant();
                                    metadata.Extensions[ext] =
                                        metadata.Extensions.GetValueOrDefault(ext) + 1;
                                }
                            }

                            yield return new Node
                            {
                                Id = _currentId++,
                                ParentId = newId,
                                Name = Path.GetFileName(filePath),
                                Type = NodeType.File,
                                Level = _flags.filesOnly ? level : level + 1,
                                Path = filePath,
                            };
                        }
                }
            }
        }
    }
}
