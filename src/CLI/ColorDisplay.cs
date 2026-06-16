using System.Linq.Expressions;

namespace CLI
{
    public class ColorDisplay
    {
        /// <summary>
        /// Writes a new line in the console following the provided foreground and background colors.
        /// </summary>
        /// <param name="content">The text to be displayed.</param>
        /// <param name="txt">The foreground/text color.</param>
        /// <param name="bkg">The background color.</param>
        /// <param name="newLine">Whether to insert a new line at the end.</param>
        static void WriteColor(
            string content,
            ConsoleColor txt = ConsoleColor.White,
            ConsoleColor bkg = ConsoleColor.Black,
            bool newLine = true
        )
        {
            Console.ForegroundColor = txt;
            Console.BackgroundColor = bkg;
            Console.Write(content + (newLine ? "\n" : null));
            Console.ResetColor();
        }

        public enum MsgType
        {
            Error,
            Warning,
            Info,
            Request,
            Success,
            Choice,
        }

        /// <summary>
        /// Writes a message in the console based on the message type chosen.
        /// </summary>
        /// <param name="msg">The message's content.</param>
        /// <param name="type">The message's type.</param>
        /// <param name="options">If 'type' is set to 'Choice', provide an array of options to choose from.</param>
        public static void WriteMsg(string msg, MsgType type, string[]? options = null)
        {
            switch (type)
            {
                case MsgType.Error:
                    WriteColor("❌ [ERROR] " + msg + "\n", ConsoleColor.Red);
                    break;

                case MsgType.Warning:
                    WriteColor("⚠️ [WARNING] " + msg + "\n", ConsoleColor.Yellow);
                    break;

                case MsgType.Info:
                    WriteColor("ℹ️ [INFO] " + msg + "\n", ConsoleColor.Blue);
                    break;

                case MsgType.Request:
                    WriteColor("➡️ " + msg + ": ", ConsoleColor.Cyan, newLine: false);
                    break;

                case MsgType.Success:
                    WriteColor("✅ [SUCCESS] " + msg + "\n", ConsoleColor.Green);
                    break;

                case MsgType.Choice:
                    WriteColor("💬 " + msg + ": ", ConsoleColor.Cyan);

                    if (options == null || options.Length == 0)
                    {
                        WriteMsg("There are no options available", MsgType.Error);
                        break;
                    }

                    int marginRight =
                        options
                            .Aggregate("", (max, cur) => cur.Length > max.Length ? cur : max)
                            .Length + 5;
                    for (int k = 0; k < options.Length; k += 3)
                    {
                        Console.Write(
                            $"[\x1b[32m{k}\u001b[0m] {options[k].Replace("_", " ").PadRight(marginRight)}"
                        );

                        if (k + 1 < options.Length)
                            Console.Write(
                                $"[\u001b[32m{k + 1}\u001b[0m] {options[k + 1].Replace("_", " ").PadRight(marginRight)}"
                            );

                        if (k + 2 < options.Length)
                            Console.Write(
                                $"[\u001b[32m{k + 2}\u001b[0m] {options[k + 2].Replace("_", " ")}"
                            );

                        Console.WriteLine();
                    }
                    break;
                default:
                    break;
            }
        }

        public static void DeleteCurrentLine()
        {
            int line = Console.CursorTop;
            Console.SetCursorPosition(0, line);
            Console.WriteLine(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, line);
        }

        /// <summary>
        /// Plays a 'spinner' loadign animation while the given Task is running.
        /// </summary>
        /// <param name="task">The Task's running instance.</param>
        /// <param name="message">The message to be displayed to the right of the spinner.</param>
        /// <param name="delayMs">The delay between spinner frame updates.</param>
        public static void LoadingAnimation(Task task, string message, int delayMs)
        {
            char[] frames = { '\\', '|', '/', '-' };
            int index = 0;
            Console.Write("  " + message);
            Console.SetCursorPosition(0, Console.CursorTop);
            while (!task.IsCompleted)
            {
                Console.Write("\r" + frames[index]);

                index = (index + 1) % frames.Length;
                Thread.Sleep(delayMs);
            }
            DeleteCurrentLine();
        }

        public static void WaitForInput()
        {
            WriteMsg("Press any key to continue", MsgType.Request);
            Console.ReadKey();
            Console.WriteLine();
        }
    }
}
