using Core.Utils;

namespace Core
{
    public class TreeGenerator
    {
        private int _currentId = 0;

        private GenFlags _flags = new();
        public Metadata metadata = new();
        public Statistics stats = new();
        public Dictionary<string, int> Extensions { get; set; } = new();
        private bool filterByWhitelist = false,
            filterByBlacklist = false;

        public TreeGenerator(GenFlags genFlags)
        {
            _flags = genFlags;
            metadata.dirPath = _flags.targetDir;
            DirectoryInfo info = new(metadata.dirPath);
            metadata.dirName = info.Name;
            metadata.genDateTime = Calendar.GetDate().Replace("-", "/") + " " + Calendar.GetTime();
            filterByWhitelist = _flags.extWhitelist.Any();
            filterByBlacklist = _flags.extBlacklist.Any();
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
            bool hasAccess = true;
            bool folderIsEmpty = false;
            IEnumerable<FileSystemInfo>? entries = null;
            try
            {
                entries = directory.EnumerateFileSystemInfos();
                folderIsEmpty = !entries.Any();
            }
            catch (UnauthorizedAccessException)
            {
                hasAccess = false;
                if (!_flags.noStatistics)
                    stats.skippedFolders++;
            }

            int newId = _currentId++;

            // send current folder
            if (!_flags.filesOnly)
            {
                yield return new Node
                {
                    Id = newId,
                    ParentId = parentId,
                    Name = directory.Name,
                    IsFile = false,
                    IsEmptyDir = folderIsEmpty,
                    IsSkipped = !hasAccess,
                    IsUnscanned = level == _flags.maxLevel - 1,
                    Level = level,
                };
            }

            if (!hasAccess || level >= _flags.maxLevel)
            {
                yield break;
            }

            // scan subfolders
            IEnumerable<DirectoryInfo>? subDirs = entries?.OfType<DirectoryInfo>();
            if (subDirs != null)
                foreach (var subDir in subDirs)
                {
                    if (FileSystem.IsReparsePoint(subDir.FullName))
                    {
                        yield return new Node
                        {
                            Id = _currentId++,
                            ParentId = newId,
                            Name =
                                (_flags.noIcons ? null : "[→] ") + subDir.Name + " (reparse point)",
                            IsFile = false,
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
                IEnumerable<FileInfo>? files = entries?.OfType<FileInfo>();
                if (files != null)
                    foreach (var file in files)
                    {
                        if (!_flags.noStatistics)
                        {
                            stats.totalSizeBytes += file.Length;
                        }

                        string ext = file.Extension.ToLowerInvariant();

                        if (filterByWhitelist && !_flags.extWhitelist.Contains(ext)) // not whitelisted
                            continue;

                        if (filterByBlacklist && _flags.extBlacklist.Contains(ext)) // blacklisted
                            continue;

                        Extensions[ext] = Extensions.GetValueOrDefault(ext) + 1;
                        // whitelisted OR not blacklisted OR lists were not defined
                        yield return new Node
                        {
                            Id = _currentId++,
                            ParentId = newId,
                            Name = file.Name,
                            IsFile = true,
                            Level = _flags.filesOnly ? level : level + 1,
                            Path = file.FullName,
                        };
                    }
            }
        }
    }
}
