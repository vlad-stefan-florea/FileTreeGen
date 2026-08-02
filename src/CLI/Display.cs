using System.Diagnostics;

namespace CLI
{
    public class Display
    {
        /// <summary>
        /// Writes a new line in the console following the provided foreground and background colors.
        /// </summary>
        /// <param name="content">The text to be displayed.</param>
        /// <param name="txt">The foreground/text color.</param>
        /// <param name="bkg">The background color.</param>
        /// <param name="newLine">Whether to insert a new line at the end.</param>
        public static void WriteColor(
            string content,
            ConsoleColor txt = ConsoleColor.White,
            ConsoleColor bkg = ConsoleColor.Black,
            bool newLine = true
        )
        {
            Console.ForegroundColor = txt;
            Console.BackgroundColor = bkg;
            Console.Write(content);
            if (newLine)
                Console.WriteLine();
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
            Save,
        }

        /// <summary>
        /// Writes a message in the console based on the message type chosen.
        /// </summary>
        /// <param name="msg">The message's content.</param>
        /// <param name="type">The message's type.</param>
        public static void WriteMsg(string msg, MsgType type)
        {
            switch (type)
            {
                case MsgType.Error:
                    WriteColor("❌ [ERROR] " + msg, ConsoleColor.Red, newLine: true);
                    break;

                case MsgType.Warning:
                    WriteColor("⚠️ [WARNING] " + msg, ConsoleColor.Yellow, newLine: true);
                    break;

                case MsgType.Info:
                    WriteColor("ℹ️ [INFO] " + msg, ConsoleColor.Blue, newLine: true);
                    break;

                case MsgType.Request:
                    WriteColor("➡️ " + msg + ": ", ConsoleColor.Cyan, newLine: false);
                    break;

                case MsgType.Success:
                    WriteColor("✅ [SUCCESS] " + msg, ConsoleColor.Green, newLine: true);
                    break;

                case MsgType.Choice:
                    WriteColor("💬 " + msg, ConsoleColor.Cyan);
                    break;

                case MsgType.Save:
                    WriteColor("💾 [SAVE] " + msg, ConsoleColor.White, newLine: true);
                    break;

                default:
                    break;
            }
        }

        public static void DeleteCurrentLine()
        {
            int line = Console.CursorTop;
            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, line);
        }

        /// <summary>
        /// Plays a 'spinner' loading animation while the given Task is running.
        /// </summary>
        /// <param name="task">The Task's running instance.</param>
        /// <param name="message">The message to be displayed to the right of the spinner.</param>
        /// <param name="delayMs">The delay between spinner frame updates.</param>
        /// <param name="showTime">Displays the elapsed time since the spinner animation has started.</param>
        public static void LoadingAnimation(Task task, string message, int delayMs, bool showTime)
        {
            char[] frames = { '\\', '|', '/', '-' };
            int index = 0;

            Stopwatch sw = new();
            if (showTime)
                sw.Start();

            Console.SetCursorPosition(0, Console.CursorTop);
            while (!task.IsCompleted)
            {
                DeleteCurrentLine();
                Console.Write(frames[index] + " " + message);
                index = (index + 1) % frames.Length;
                if (showTime)
                    Console.Write($" ({sw.Elapsed.TotalSeconds.ToString("#0.0")}s)");
                Thread.Sleep(delayMs);
            }
            DeleteCurrentLine();
            if (showTime)
            {
                WriteMsg(
                    $"Operation finished in {sw.Elapsed.TotalSeconds.ToString("#0.0")}s",
                    MsgType.Info
                );
                sw.Stop();
            }
        }

        public static void WaitForInput()
        {
            WriteMsg("Press any key to continue", MsgType.Request);
            Console.ReadKey();
            Console.WriteLine();
        }
    }
}
