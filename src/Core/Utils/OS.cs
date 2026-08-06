using System.Security.Principal;
using static Core.Settings;

namespace Core.Utils
{
    public class OS
    {
        public static PrivilegeLevel GetPrivilegeLevel()
        {
            if (!OperatingSystem.IsWindows())
                return PrivilegeLevel.Unknown;
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator)
                ? PrivilegeLevel.Elevated
                : PrivilegeLevel.Standard;
        }
    }
}
