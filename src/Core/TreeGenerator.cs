using Core.Utils;

namespace Core
{
    public class TreeGenerator
    {
        private int _currentId = 0;

        private GenFlags _flags = new();
        public Metadata metadata = new();
        public Statistics stats = new();
        public Dictionary<string, int> Extensions { get; set; } =
            new(StringComparer.OrdinalIgnoreCase);
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
                bool hasContents = Directory.EnumerateFileSystemEntries(_flags.targetDir).Any();
                if (!hasContents)
                    throw new CoreException(
                        ExitCode.RootIsEmpty,
                        $"The target directory ({_flags.targetDir}) was found empty when attempted to generate the report."
                    );
                return TraverseDirectory(new DirectoryInfo(_flags.targetDir), null, 0);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CoreException(ExitCode.RootAccessDenied, ex.Message);
                // if not, throw an error and don't generate any report
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
                if (_flags.includeStatistics)
                    stats.skippedFolders++;
            }

            int newId = _currentId++;

            // send current folder
            if (!_flags.filesOnly && !(_flags.ignoreEmptyDirs && folderIsEmpty))
            {
                yield return new Node
                {
                    Id = newId,
                    ParentId = parentId,
                    Name = _flags.nodeLabel switch
                    {
                        Settings.NodeLabel.Full_Path => directory.FullName,
                        Settings.NodeLabel.Relative_Path => Path.GetRelativePath(
                            _flags.targetDir,
                            directory.FullName
                        ),
                        _ => directory.Name, // .Name included
                    },
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
                        if (_flags.includeStatistics)
                            stats.skippedFolders++;
                        if (_flags.filesOnly)
                            continue;
                        yield return new Node
                        {
                            Id = _currentId++,
                            ParentId = newId,
                            Name =
                                (!_flags.includeIcons ? null : "[→] ")
                                + _flags.nodeLabel switch
                                {
                                    Settings.NodeLabel.Full_Path => subDir.FullName,
                                    Settings.NodeLabel.Relative_Path => Path.GetRelativePath(
                                        _flags.targetDir,
                                        subDir.FullName
                                    ),
                                    _ => subDir.Name, // .Name included
                                }
                                + " (reparse point)",
                            IsFile = false,
                            IsSkipped = true,
                            Level = level + 1,
                        };
                    }
                    else
                    {
                        if (_flags.includeStatistics)
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
                {
                    foreach (var file in files)
                    {
                        string ext = file.Extension;
                        bool skipped = false;
                        if (filterByWhitelist && !_flags.extWhitelist.Contains(ext))
                            skipped = true;
                        if (filterByBlacklist && _flags.extBlacklist.Contains(ext))
                            skipped = true;
                        if (_flags.includeStatistics)
                        {
                            if (skipped)
                                stats.skippedFiles++;
                            else
                                stats.totalSizeBytes += file.Length;
                        }
                        if (skipped)
                            continue;

                        Extensions[ext] = Extensions.GetValueOrDefault(ext) + 1;
                        yield return new Node
                        {
                            Id = _currentId++,
                            ParentId = _flags.filesOnly ? 0 : newId,
                            Name = _flags.nodeLabel switch
                            {
                                Settings.NodeLabel.Full_Path => file.FullName,
                                Settings.NodeLabel.Relative_Path => Path.GetRelativePath(
                                    _flags.targetDir,
                                    file.FullName
                                ),
                                _ => file.Name, // .Name included
                            },
                            IsFile = true,
                            Level = _flags.filesOnly ? 0 : level + 1,
                            Path = file.FullName,
                        };
                    }
                }
            }
        }
    }
}
