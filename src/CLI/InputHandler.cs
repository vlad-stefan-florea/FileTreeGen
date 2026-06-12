using static CLI.ColorDisplay;
using static Core.NodeGenerator; // for 'OutputFormat'
using static Core.Utils.Text;

namespace CLI
{
    public class InputHandler
    {
        private static int AttemptLimit = 10;

        public static string AskForDir()
        {
            int counter = 0;
            string dir = string.Empty;

            while (string.IsNullOrEmpty(dir))
            {
                counter++;
                if (counter % AttemptLimit == 0)
                    Console.Clear();

                WriteMsg("Directory path", MsgType.Request);
                dir = CleanPath(Console.ReadLine());

                if (string.IsNullOrEmpty(dir))
                    WriteMsg($"Please drag & drop the folder or fill in its path", MsgType.Info);
                else if (!Directory.Exists(dir))
                {
                    WriteMsg($"The folder does not exist: {dir}", MsgType.Error);
                    dir = string.Empty;
                }
            }
            return dir;
        }

        public static OutputFormat AskForFormat()
        {
            int choice = -1,
                counter = 0;

            var options = new string[] { "HTML", "Markdown", "Text" };
            WriteMsg(
                "Please choose the number of the preferred output format",
                MsgType.Choice,
                Options: options
            );

            while (true)
            {
                counter++;
                if (counter % AttemptLimit == 0)
                {
                    Console.Clear();
                    WriteMsg(
                        "Please choose the number of the preferred output format",
                        MsgType.Choice,
                        Options: options
                    );
                }

                WriteMsg("Choice", MsgType.Request);
                string? ans = Console.ReadLine();
                if (!string.IsNullOrEmpty(ans))
                {
                    choice = Convert.ToInt32(ans);
                    switch (choice)
                    {
                        case 0:
                            return OutputFormat.HTML;
                        case 1:
                            return OutputFormat.Markdown;
                        case 2:
                            return OutputFormat.Text;
                        default:
                            break;
                    }
                }
                WriteMsg("Please chose one of the options above", MsgType.Info);
            }
        }
    }
}
