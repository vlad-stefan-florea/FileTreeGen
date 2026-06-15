using System.Diagnostics.Metrics;
using static CLI.ColorDisplay;
using static Core.Settings;
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

        public static bool AskYN(string prompt)
        {
            int counter = 0;
            string ans = string.Empty;

            while (string.IsNullOrEmpty(ans))
            {
                counter++;
                if (counter % AttemptLimit == 0)
                    Console.Clear();

                WriteMsg($"{prompt}\n[Y/N]", MsgType.Request);
                ans = Console.ReadLine().Trim().ToLower();

                if (string.IsNullOrEmpty(ans))
                {
                    WriteMsg($"Please answer with 'y' for YES or 'n' for NO", MsgType.Info);
                    ans = string.Empty;
                }
            }
            if (ans == "y")
                return true;
            else
                return false;
        }

        public static T ChoiceMenu<T>(string prompt)
        {
            int counter = 0;
            string[] options = Enum.GetNames(typeof(T));
            T[] values = (T[])Enum.GetValues(typeof(T));

            WriteMsg(prompt, MsgType.Choice, options);

            while (true)
            {
                counter++;
                if (counter % AttemptLimit == 0)
                {
                    Console.Clear();
                    WriteMsg(prompt, MsgType.Choice, options);
                }

                WriteMsg("Choice", MsgType.Request);
                string? ans = Console.ReadLine();

                if (int.TryParse(ans, out int choice))
                    if (choice >= 0 && choice < values.Length)
                        return values[choice];
            }
        }
    }
}
