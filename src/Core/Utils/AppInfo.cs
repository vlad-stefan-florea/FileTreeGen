using System.Reflection;

namespace Core.Utils
{
    public static class AppInfo
    {
        public const string AppName = "FileTreeGen";
        public const string AppUrl = "https://github.com/vlad-stefan-florea/FileTreeGen";
        public const string Developer = "@vlad-stefan-florea";
        public static string Version { get; }

        static AppInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var v = assembly.GetName().Version;
            Version = v != null ? $"{v.Major}.{v.Minor}.{v.Build}" : "0.0.0";
        }
    }
}
