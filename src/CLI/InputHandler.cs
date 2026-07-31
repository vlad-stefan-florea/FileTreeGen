using static CLI.ColorDisplay;
using static Core.Utils.Text;

namespace CLI
{
    public class InputHandler
    {
        private const int waitTimeMs = 1500;
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
            WriteMsg($"Directory chosen: '{dir}'", MsgType.Save);
            return CleanPath(dir);
        }

        /// <summary>
        /// Displays a YES/NO prompt.
        /// </summary>
        /// <param name="prompt">The prompt to be displayed.</param>
        /// <param name="defaultValue">The default value for the prompt.</param>
        /// <returns>A bool based on the answer (true for YES; false for NO).</returns>
        public static bool AskYN(string prompt, bool defaultValue)
        {
            string? ans = string.Empty;
            string[] options = { "YES", "NO" };
            int maxId = options.Length - 1,
                selectedId = 0;
            bool selected = false;
            Thread.Sleep(waitTimeMs);
            while (!selected)
            {
                Console.Clear();
                WriteMsg(prompt, MsgType.Choice);
                WriteMsg(
                    "↑/↓ or 'y'/'n' Navigate | 'Enter' Select | 'Esc'/'Backspace' Cancel",
                    MsgType.Info
                );
                for (int i = 0; i <= maxId; i++)
                {
                    if (i == selectedId)
                        WriteColor("> " + options[i], ConsoleColor.Black, ConsoleColor.White);
                    else
                        Console.WriteLine("  " + options[i]);
                }
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Backspace:
                        return defaultValue;
                    case ConsoleKey.Escape:
                        return defaultValue;
                    case ConsoleKey.Enter:
                        selected = true;
                        break;
                    case ConsoleKey.UpArrow:
                        if (selectedId > 0)
                            selectedId--;
                        else
                            selectedId = maxId;
                        break;
                    case ConsoleKey.DownArrow:
                        if (selectedId < maxId)
                            selectedId++;
                        else
                            selectedId = 0;
                        break;
                    case ConsoleKey.Y:
                        selectedId = 0;
                        selected = true;
                        break;
                    case ConsoleKey.N:
                        selectedId = 1;
                        selected = true;
                        break;

                    default:
                        break;
                }
            }
            WriteMsg("Selected: " + (selectedId == 0 ? "YES" : "NO"), MsgType.Save);
            return selectedId == 0 ? true : false; // YES -> true | NO -> false
        }

        /// <summary>
        /// Asks for an integer value between the 'min' and 'max' provided values.
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
            if (AskYN(prompt, currentValue))
                currentValue = true;
            else
                currentValue = false;
            WriteMsg("Updated value: " + currentValue, MsgType.Save);
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
            string[] options = Enum.GetNames(typeof(T));
            for (int i = 0; i < options.Length; i++)
                options[i] = options[i].Replace("_", " ");

            T[] values = Enum.GetValues<T>();

            int maxId = options.Length - 1,
                selectedId = 0;
            bool selected = false;

            Thread.Sleep(waitTimeMs);
            while (!selected)
            {
                Console.Clear();
                WriteMsg(prompt, MsgType.Choice);
                WriteMsg("↑/↓ Navigate | 'Enter' Select | 'Esc'/'Backspace' Cancel", MsgType.Info);

                for (int i = 0; i <= maxId; i++)
                {
                    if (i == selectedId)
                        WriteColor($"> {i}. {options[i]}", ConsoleColor.Black, ConsoleColor.White);
                    else
                        Console.WriteLine($"  {i}. " + options[i]);
                }

                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Backspace:
                        return null;
                    case ConsoleKey.Escape:
                        return null;
                    case ConsoleKey.Enter:
                        selected = true;
                        break;
                    case ConsoleKey.UpArrow:
                        if (selectedId > 0)
                            selectedId--;
                        else
                            selectedId = maxId;
                        break;
                    case ConsoleKey.DownArrow:
                        if (selectedId < maxId)
                            selectedId++;
                        else
                            selectedId = 0;
                        break;
                    default:
                        break;
                }
            }
            WriteMsg(
                $"Selected: '{values[selectedId].ToString().Replace("_", " ")}'",
                MsgType.Save
            );
            return values[selectedId];
        }
    }
}
