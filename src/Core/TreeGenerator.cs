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
            metadata.privileges = OS.GetPrivilegeLevel();
            filterByWhitelist = _flags.extWhitelist.Any();
            filterByBlacklist = _flags.extBlacklist.Any();
        }

        public IEnumerable<Node> GenerateTree()
        {
            _currentId = 0;
            try
            {
                // test if the root folder is valid, accessible and not empty
                bool hasContents = Directory.EnumerateFileSystemEntries(_flags.targetDir).Any();
                if (!hasContents)
                    throw new CoreException(
                        ExitCode.TargetIsEmpty,
                        ExitMessages.Get(ExitCode.TargetIsEmpty)
                    );
                return TraverseDirectory(new DirectoryInfo(_flags.targetDir), null, 0);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CoreException(
                    ExitCode.TargetAccessDenied,
                    ExitMessages.Get(ExitCode.TargetAccessDenied),
                    ex
                );
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new CoreException(
                    ExitCode.TargetNotFound,
                    ExitMessages.Get(ExitCode.TargetNotFound),
                    ex
                );
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
            if (!(_flags.ignoreEmptyDirs && folderIsEmpty))
            {
                yield return new Node
                {
                    Id = newId,
                    ParentId = parentId,
                    Name = _flags.nodeLabel switch
                    {
                        Settings.NodeLabel.FullPath => directory.FullName,
                        Settings.NodeLabel.RelativePath => Path.GetRelativePath(
                            _flags.targetDir,
                            directory.FullName
                        ),
                        _ => directory.Name, // .Name included
                    },
                    IsFile = false,
                    IsEmptyDir = folderIsEmpty,
                    IsSkipped = !hasAccess,
                    IsUnscanned = level == _flags.maxDepth - 1,
                    Level = level,
                };
            }

            if (!hasAccess || level >= _flags.maxDepth)
            {
                yield break;
            }

            // scan subfolders
            if (!_flags.filesOnly)
            {
                IEnumerable<DirectoryInfo>? subDirs = entries?.OfType<DirectoryInfo>();
                if (subDirs != null)
                {
                    foreach (var subDir in subDirs)
                    {
                        if (_flags.includeStatistics)
                            stats.folders++;
                        if (FileSystem.IsReparsePoint(subDir.FullName) && !_flags.ignoreSymlinks)
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
                                        Settings.NodeLabel.FullPath => subDir.FullName,
                                        Settings.NodeLabel.RelativePath => Path.GetRelativePath(
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
                            if (_flags.dirBlacklist.Contains(subDir.Name))
                            {
                                if (_flags.includeStatistics)
                                    stats.skippedFolders++;
                                continue;
                            }
                            foreach (var childNode in TraverseDirectory(subDir, newId, level + 1))
                            {
                                yield return childNode;
                            }
                        }
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
                        Extensions[ext] = Extensions.GetValueOrDefault(ext) + 1;
                        if (skipped)
                            continue;
                        yield return new Node
                        {
                            Id = _currentId++,
                            ParentId = _flags.filesOnly ? 0 : newId,
                            Name = _flags.nodeLabel switch
                            {
                                Settings.NodeLabel.FullPath => file.FullName,
                                Settings.NodeLabel.RelativePath => Path.GetRelativePath(
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
