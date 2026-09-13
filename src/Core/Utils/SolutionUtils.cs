namespace Core.Utils
{
    public class SolutionUtils
    {
        public static DirectoryInfo GetSolutionRoot()
        {
            string[] solutionExts = [".sln", ".slnx"];
            var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
            while (currentDir != null)
            {
                foreach (var file in currentDir.GetFiles())
                    if (solutionExts.Contains(file.Extension, StringComparer.OrdinalIgnoreCase))
                        return currentDir;
                currentDir = currentDir.Parent;
            }
            throw new DirectoryNotFoundException("The Solution's root was not found (.sln/.slnx).");
        }

        public static string GetFTGRootPath()
        {
            DirectoryInfo rootDir = GetSolutionRoot().Parent;
            if (rootDir != null)
                return rootDir.FullName;
            else
                throw new DirectoryNotFoundException(
                    "The FileTreeGen's root folder couldn't be accessed."
                );
        }
    }
}
