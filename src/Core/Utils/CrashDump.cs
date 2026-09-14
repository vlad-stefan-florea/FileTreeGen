namespace Core.Utils
{
    public class CrashDump
    {
        static string crashDir = Path.Combine(AppInfo.AppDir, "ftg_crash_dump");

        public static (bool Success, string? FilePath) GenerateDump(CoreException ex)
        {
            string filePath = Path.Combine(
                crashDir,
                "crash-"
                    + Calendar.GetDateReversed().Replace("-", "")
                    + '-'
                    + Calendar.GetTime().Replace(":", "").Replace(".", "")
                    + ".log"
            );
            try
            {
                if (!Directory.Exists(crashDir))
                    Directory.CreateDirectory(crashDir);
                File.WriteAllText(filePath, ex.ToString());
            }
            catch (Exception)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return (false, null);
            }
            return (true, filePath);
        }

        public static bool CleanCrashDump()
        {
            try
            {
                if (Directory.Exists(crashDir))
                    Directory.Delete(crashDir, true);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
