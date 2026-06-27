using static CLI.ColorDisplay;
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
                else if (!Directory.EnumerateFileSystemEntries(dir).Any())
                {
                    WriteMsg($"The folder cannot be empty: {dir}", MsgType.Error);
                    dir = string.Empty;
                }
            }
            WriteMsg($"Directory chosen: '{dir}'", MsgType.Success);
            return CleanPath(dir);
        }

        /// <summary>
        /// Displays a YES/NO prompt.
        /// </summary>
        /// <param name="prompt">The prompt to be displayed.</param>
        /// <returns>A bool based on the answer (true for YES; false for NO).</returns>
        public static bool AskYN(string prompt)
        {
            string? ans = string.Empty;
            WriteMsg($"{prompt}\n[Y/N]", MsgType.Request);
            ans = Console.ReadLine()?.Trim().ToLower();
            if (ans == "y")
            {
                Console.WriteLine("(ANS: YES)");
                return true;
            }
            else
            {
                Console.WriteLine("(ANS: NO)");
                return false;
            }
        }

        /// <summary>
        /// Asks for an integer value between the 'min' and 'max' provied values.
        /// </summary>
        /// <param name="prompt">The prompt to be displayed.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns></returns>
        public static int AskForInt(string prompt, int min, int max)
        {
            string? ans = string.Empty;
            int counter = 0;
            while (true)
            {
                counter++;
                if (counter % AttemptLimit == 0)
                {
                    Console.Clear();
                    WriteMsg($"{prompt} [{min}<->{max}]", MsgType.Request);
                }
                WriteMsg($"{prompt} [{min}<->{max}]", MsgType.Request);
                ans = Console.ReadLine()?.Trim().ToLower();
                if (!string.IsNullOrEmpty(ans) && int.TryParse(ans, out int n))
                {
                    if (n >= min && n <= max)
                        return n;
                    else
                    {
                        WriteMsg(
                            $"Please enter a value between {min} and {max} (including both)",
                            MsgType.Info
                        );
                        continue;
                    }
                }
                else
                    continue;
            }
        }

        public static HashSet<string> AskForExtensions(
            string prompt,
            HashSet<string> currentExtensions
        )
        {
            string? ans = string.Empty;
            int counter = 0;
            while (true)
            {
                counter++;
                if (counter % AttemptLimit == 0)
                {
                    Console.Clear();
                    WriteMsg($"{prompt}", MsgType.Request);
                }
                WriteMsg($"{prompt}", MsgType.Request);
                ans = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(ans))
                {
                    WriteMsg("Operation canceled", MsgType.Warning);
                    WriteMsg("Use '--clear' to clear the list", MsgType.Info);
                    return currentExtensions;
                }
                else
                {
                    if (ans.Contains("--clear"))
                        return new HashSet<string>();
                    var result = Core.Utils.ListParser.ParseExtensionList(ans);
                    if (result.Any())
                    {
                        currentExtensions.UnionWith(result);
                        return currentExtensions;
                    }
                    else
                    {
                        WriteMsg(
                            "Please write the extensions as in the following examples:"
                                + "\n\t- For simple extensions: .txt;.docx;.html;.exe"
                                + "\n\t- For no/empty extensions, use '\"\"' OR 'none'"
                                + "\n\t- The separator characters can be either ';' or ','"
                                + "\n\t- Extensions starting with '.' is optional"
                                + "\n\t- Use '--clear' to clear the list",
                            MsgType.Info
                        );
                    }
                }
            }
        }

        public static bool AskForSwitch(string prompt, bool currentValue)
        {
            WriteMsg("Current value: " + currentValue, MsgType.Info);
            if (AskYN(prompt))
                currentValue = true;
            else
                currentValue = false;
            WriteMsg("Updated value: " + currentValue, MsgType.Success);
            return currentValue;
        }

        /// <summary>
        /// Displays a choice menu based on the given structure.
        /// </summary>
        /// <param name="T">The Enum containing all the available options.</typeparam>
        /// <param name="prompt">The prompt to be displayed.</param>
        /// <returns>Returns the chosen Enum entry.</returns>
        public static T? ChoiceMenu<T>(string prompt)
            where T : struct, Enum
        {
            int counter = 0;
            string[] options = Enum.GetNames(typeof(T));
            T[] values = Enum.GetValues<T>();

            WriteMsg(prompt + " ('q' to quit)", MsgType.Choice, options);

            while (true)
            {
                counter++;
                if (counter % AttemptLimit == 0)
                {
                    Console.Clear();
                    WriteMsg(prompt + " ('q' to quit)", MsgType.Choice, options);
                }

                WriteMsg("Choice", MsgType.Request);
                string? ans = Console.ReadLine();

                if (int.TryParse(ans, out int choice) && choice >= 0 && choice < values.Length)
                {
                    WriteMsg(
                        $"Option chosen: '{values[choice].ToString().Replace("_", " ")}'",
                        MsgType.Success
                    );
                    return values[choice];
                }
                else if (string.Equals(ans, "q", StringComparison.OrdinalIgnoreCase))
                    return null;
            }
        }
    }
}
