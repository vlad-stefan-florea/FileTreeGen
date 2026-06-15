namespace Core
{
    public class TreeGenerator
    {
        private int _currentId = 0;
        public int skippedFolders = 0,
            skippedFiles = 0;

        public IEnumerable<Node> GenerateTree(string rootPath)
        {
            _currentId = 0;
            return TraverseDirectory(new DirectoryInfo(rootPath), null, 0);
        }

        private IEnumerable<Node> TraverseDirectory(
            DirectoryInfo directory,
            int? parentId,
            int level
        )
        {
            int newId = _currentId++;

            // send current folder
            yield return new Node
            {
                Id = newId,
                ParentId = parentId,
                Name = directory.Name,
                Type = NodeType.Folder,
                Level = level,
            };

            // scan subfolders
            DirectoryInfo[] subDirs = [];
            try
            {
                subDirs = directory.GetDirectories();
            }
            catch
            {
                // Ignore
                skippedFolders++;
            }

            foreach (var subDir in subDirs)
            {
                foreach (var childNode in TraverseDirectory(subDir, newId, level + 1))
                {
                    yield return childNode;
                }
            }

            // send files
            foreach (var filePath in Directory.EnumerateFiles(directory.FullName))
            {
                yield return new Node
                {
                    Id = _currentId++,
                    ParentId = newId,
                    Name = Path.GetFileName(filePath),
                    Type = NodeType.File,
                    Level = level + 1,
                    Path = filePath,
                };
            }
        }
    }
}
