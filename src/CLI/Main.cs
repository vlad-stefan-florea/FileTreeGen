using static CLI.ColorDisplay;
using static Core.NodeGenerator; // for 'OutputFormat'

namespace CLI
{
    public class Main
    {
        public static void Run()
        {
            string dir = InputHandler.AskForDir();
            Console.Clear();
            WriteMsg($"Directory chosen: '{dir}'", MsgType.Success);

            OutputFormat format = InputHandler.AskForFormat();
            Console.Clear();
            WriteMsg($"Format chosen: '{format}'", MsgType.Success);
        }
    }
}
