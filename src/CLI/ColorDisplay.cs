using System.Linq.Expressions;

namespace CLI
{
    public class ColorDisplay
    {
        static void WriteColor(
            string Content,
            ConsoleColor Txt = ConsoleColor.White,
            ConsoleColor Bkg = ConsoleColor.Black,
            bool NewLine = true
        )
        {
            Console.ForegroundColor = Txt;
            Console.BackgroundColor = Bkg;
            Console.Write(Content + (NewLine ? "\n" : ""));
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

        public static void WriteMsg(string Msg, MsgType Type, string[]? Options = null)
        {
            const int choiceSpaceWidth = 20;
            switch (Type)
            {
                case MsgType.Error:
                    WriteColor("❌ " + Msg + "\n", ConsoleColor.Red);
                    break;

                case MsgType.Warning:
                    WriteColor("⚠️ " + Msg + "\n", ConsoleColor.Yellow);
                    break;

                case MsgType.Info:
                    WriteColor("ℹ️ " + Msg + "\n", ConsoleColor.Blue);
                    break;

                case MsgType.Request:
                    WriteColor("➡️ " + Msg + ": ", ConsoleColor.Cyan, NewLine: false);
                    break;

                case MsgType.Success:
                    WriteColor("✅ " + Msg + "\n", ConsoleColor.Green);
                    break;

                case MsgType.Choice:
                    WriteColor("💬 " + Msg + ": ", ConsoleColor.Cyan);

                    if (Options == null || Options.Length == 0)
                    {
                        WriteMsg("There are no options available", MsgType.Error);
                        break;
                    }

                    for (int k = 0; k < Options.Length; k += 3)
                    {
                        Console.Write($"[\x1b[32m{k}\u001b[0m] {Options[k], -choiceSpaceWidth}");

                        if (k + 1 < Options.Length)
                            Console.Write(
                                $"[\u001b[32m{k + 1}\u001b[0m] {Options[k + 1], -choiceSpaceWidth}"
                            );

                        if (k + 2 < Options.Length)
                            Console.Write($"[\u001b[32m{k + 2}\u001b[0m] {Options[k + 2]}");

                        Console.WriteLine();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
