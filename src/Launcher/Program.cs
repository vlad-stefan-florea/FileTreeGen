using static CLI.ColorDisplay;
using static Core.Utils.Text;

namespace Launcher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool CliNeeded = true;
            string? errMsg = null;

            if (args.Length > 0)
                if (Directory.Exists(CleanPath(args[0])))
                    CliNeeded = false;
                else
                    errMsg = $"The specified directory does not exist: {args[0]}";

            if (CliNeeded)
            {
                if (errMsg != null)
                    WriteMsg(errMsg, MsgType.Error);
                await CLI.Main.Run();
            }
            else
                Console.WriteLine("Silent can be run");
        }
    }
}
