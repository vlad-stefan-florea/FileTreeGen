using Core.Utils;
using static Core.Utils.Text;
using static TUI.Display;

namespace TUI
{
    public class InputHandler
    {
        private const int maxDisplayedOptions = 10;

        /// <summary>
        /// Prompts the user to enter an existing directory path.
        /// </summary>
        /// <param name="canBeEmpty">Indicates whether the selected directory is allowed to be empty.</param>
        /// <returns>The validated directory path.</returns>
        public static string AskForDir(bool canBeEmpty = true)
        {
            string dir = string.Empty;

            while (string.IsNullOrWhiteSpace(dir))
            {
                WriteMsg("Directory path", MsgType.Request);
                dir = CleanPath(Console.ReadLine());

                if (string.IsNullOrWhiteSpace(dir))
                    WriteMsg($"Please drag & drop the folder or fill in its path", MsgType.Info);
                else if (!Directory.Exists(dir))
                {
                    WriteMsg($"The folder does not exist: {dir}", MsgType.Warning);
                    dir = string.Empty;
                }
                else if (!canBeEmpty && !Directory.EnumerateFileSystemEntries(dir).Any())
                {
                    WriteMsg($"The folder cannot be empty: {dir}", MsgType.Warning);
                    dir = string.Empty;
                }
            }
            WriteMsg($"Directory chosen: '{dir}'", MsgType.Save);
            return dir;
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
            bool selected = false;
            int selectedId = defaultValue ? 0 : 1;
            // YES   /   NO
            // ^0        ^1
            // ^true     ^false
            WriteMsg(prompt, MsgType.Choice);
            WriteMsg(
                "←/→ or 'y'/'n' Navigate | 'Enter' Select | 'Esc'/'Backspace' Default",
                MsgType.Info
            );
            Console.WriteLine();
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            while (!selected)
            {
                DeleteCurrentLine();
                if (selectedId == 0)
                    WriteColor("[ YES ]", txt: ConsoleColor.Green, newLine: false);
                else
                    Console.Write(" YES ");
                Console.Write(" / ");
                if (selectedId == 1)
                    WriteColor("[ NO ]", txt: ConsoleColor.Red, newLine: false);
                else
                    Console.Write(" NO ");

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
                    case ConsoleKey.LeftArrow:
                        if (selectedId == 0)
                            selectedId = 1;
                        else
                            selectedId = 0;
                        break;
                    case ConsoleKey.RightArrow:
                        if (selectedId == 1)
                            selectedId = 0;
                        else
                            selectedId = 1;
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
            Console.WriteLine();
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
            while (true)
            {
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

        public static HashSet<string> AskForHashSet(
            string prompt,
            HashSet<string> currentHashSet,
            HashSetParser.HashSetType type
        )
        {
            string? ans = string.Empty;
            while (true)
            {
                WriteMsg($"{prompt}", MsgType.Request);
                ans = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(ans))
                {
                    WriteMsg("Operation canceled", MsgType.Warning);
                    return currentHashSet;
                }
                else
                {
                    if (ans.Contains("--clear"))
                        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    var result = HashSetParser.FromString(ans, type);
                    if (result.Count > 0)
                    {
                        currentHashSet.UnionWith(result);
                        return currentHashSet;
                    }
                }
            }
        }

        public static bool AskForSwitch(string prompt, bool currentValue)
        {
            if (AskYN(prompt + $" (Current Value: {(currentValue ? "YES" : "NO")})", currentValue))
                currentValue = true;
            else
                currentValue = false;
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
            // options setup & sorting
            string[] options = Enum.GetNames(typeof(T));
            var values = Enum.GetValues<T>();

            // variables
            int maxId = options.Length - 1,
                selectedId = 0,
                availableLines = Console.WindowHeight,
                displayedItems = Math.Min(
                    maxDisplayedOptions,
                    Math.Min(Console.WindowHeight, maxId + 1)
                ),
                firstShownId = 0,
                menuTop;
            bool selected = false;

            // small visual tweak
            for (int i = 0; i <= maxId; i++)
                options[i] = options[i].Replace('_', ' ');

            // prompt & nav info
            WriteMsg(prompt, MsgType.Choice);
            WriteMsg("↑/↓ Navigate | 'Enter' Select | 'Esc'/'Backspace' Cancel", MsgType.Info);

            // reserve lines
            for (int i = 0; i < displayedItems; i++)
                Console.WriteLine();
            Console.SetCursorPosition(0, Console.CursorTop - displayedItems);
            menuTop = Console.CursorTop;

            // selection loop
            while (!selected)
            {
                DrawOptions();

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
                    {
                        if (selectedId > 0)
                            selectedId--;
                        else
                            selectedId = maxId;
                        break;
                    }
                    case ConsoleKey.DownArrow:
                    {
                        if (selectedId < maxId)
                            selectedId++;
                        else
                            selectedId = 0;
                        break;
                    }
                    case ConsoleKey.Home:
                        selectedId = 0;
                        break;
                    case ConsoleKey.End:
                        selectedId = maxId;
                        break;
                    default:
                        break;
                }
                UpdateOptions();
            }

            // return last selected value
            return values[selectedId];

            // helper methods
            void DrawLine(int optionId)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
                string option = options[optionId],
                    counter = $"[{optionId + 1}/{maxId + 1}]";
                int padding = Console.WindowWidth - option.Length - counter.Length - 3;
                if (padding < 1)
                    padding = 1;
                string separator = optionId == selectedId
                        ? new string('.', padding)
                        : new string(' ', padding),
                    text = option + separator + counter;
                if (optionId == selectedId)
                    WriteColor("> " + text, txt: ConsoleColor.White);
                else
                    WriteColor("  " + text, txt: ConsoleColor.DarkGray);
            }
            void DrawOptions()
            {
                Console.SetCursorPosition(0, menuTop);
                for (int i = firstShownId; i < (firstShownId + displayedItems) && i <= maxId; i++)
                    DrawLine(i);
            }
            void UpdateOptions()
            {
                if (selectedId < firstShownId)
                {
                    firstShownId = selectedId;
                }
                else if (selectedId >= firstShownId + displayedItems)
                {
                    firstShownId = selectedId - displayedItems + 1;
                }
            }
        }
    }
}
