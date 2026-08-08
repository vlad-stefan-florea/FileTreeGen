namespace Benchmark
{
    public class StructureBuilder
    {
        static List<string> allDirs = new();
        static int minDirs = 2,
            maxDirs = 6,
            createdDirs = 0,
            targetDirs;
        static int createdFiles = 0,
            targetFiles;
        static string[] extensions =
        [
            ".txt",
            ".docx",
            ".pdf",
            ".csv",
            ".mp3",
            ".mp4",
            ".png",
            ".jpg",
            ".jpeg",
            ".gif",
        ];

        public static string Build(string rootPath, string testName, int files, int dirs, int depth)
        {
            //reset counters
            allDirs.Clear();
            createdDirs = 0;
            createdFiles = 0;
            targetDirs = dirs;
            targetFiles = files;
            // root dir setup
            string testPath = Path.Combine(rootPath, testName);

            if (Directory.Exists(testPath))
                Directory.Delete(testPath, true);
            Directory.CreateDirectory(testPath);
            allDirs.Add(testPath);

            BuildDirs(testPath, depth);
            PopulateDirs();
            return testPath;
        }

        static void BuildDirs(string rootPath, int maxDepth)
        {
            Queue<(string path, int depth)> queue = new();
            queue.Enqueue((rootPath, 0));
            while (queue.Count > 0 && createdDirs < targetDirs)
            {
                var (current, depth) = queue.Dequeue();
                if (depth >= maxDepth)
                    continue;

                int children = Math.Min(GetRandomInt(minDirs, maxDirs), targetDirs - createdDirs);
                for (int i = 0; i < children; i++)
                {
                    string child = Path.Combine(current, $"d{createdDirs + 1}");
                    Directory.CreateDirectory(child);
                    allDirs.Add(child);
                    createdDirs++;
                    queue.Enqueue((child, depth + 1));
                }
            }
        }

        static void PopulateDirs()
        {
            while (createdFiles < targetFiles)
            {
                string dir = allDirs[random.Next(allDirs.Count)];
                string file = Path.Combine(
                    dir,
                    $"f{createdFiles + 1}" + extensions[random.Next(extensions.Length)]
                );
                File.Create(file).Dispose();
                createdFiles++;
            }
        }

        // HELPERS
        static Random random = new Random();

        static int GetRandomInt(int min, int max) => random.Next(min, max + 1);
    }
}
