using Core;
using Core.Utils;
using static TUI.Display;
using static TUI.InputHandler;

namespace Launcher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            CoreException _ex = new(ExitCode.Success, ExitMessages.Get(ExitCode.Success));
            (CoreException Ex, List<string> Messages, Settings.Verbosity Verbo) CLIResult = new();
            bool needsTUI = args.Length == 0;
            try
            {
                if (needsTUI)
                {
                    await TUI.Main.Run();
                }
                else
                {
                    CLIResult = await CLI.Main.Run(args);
                    _ex = CLIResult.Ex;
                }
            }
            catch (CoreException ex)
            {
                _ex = ex;
            }
            catch (Exception newEx)
            {
                _ex = ExitMessages.TranslateOSException(newEx);
            }
            finally
            {
                if (_ex.Code != ExitCode.Success)
                {
                    var dump = CrashDump.GenerateDump(_ex);
                    string? trace = _ex.InnerException?.StackTrace ?? _ex.StackTrace;

                    if (needsTUI)
                    {
                        WriteMsg($"[{(int)_ex.Code} ({_ex.Code})]: {_ex.Message}", MsgType.Error);
                        bool errDetails = AskYN(
                            "Do you want to see the detailed error information?",
                            false
                        );
                        if (errDetails)
                        {
                            if (dump.Success)
                                WriteMsg($"Crash dump saved at: '{dump.FilePath}'", MsgType.Info);
                            WriteColor(
                                "[CODE]: " + (int)_ex.Code + $" ({_ex.Code})",
                                ConsoleColor.Red
                            );
                            WriteColor("[DESCRIPTION]: " + _ex.Message, ConsoleColor.Red);
                            if (trace != null)
                            {
                                WriteColor("[STACK TRACE]:\n" + trace, ConsoleColor.Red);
                            }
                            if (_ex.InnerException != null)
                            {
                                WriteColor(
                                    "[INNER MESSAGE]:\n" + _ex.InnerException.Message,
                                    ConsoleColor.Red
                                );
                            }
                        }
                    }
                    else
                    {
                        if (CLIResult.Verbo != Settings.Verbosity.Quiet) // normal OR verbose
                        {
                            WriteColor(
                                "[ERROR]: " + (int)_ex.Code + $" ({_ex.Code})",
                                ConsoleColor.Red
                            );
                            WriteColor("[DESCRIPTION]: " + _ex.Message, ConsoleColor.Red);

                            if (CLIResult.Verbo != Settings.Verbosity.Normal) // verbose
                            {
                                WriteColor("[DETAILS]:", ConsoleColor.Blue);
                                if (CLIResult.Messages != null && CLIResult.Messages.Count > 0)
                                {
                                    int c = 1;
                                    foreach (var err in CLIResult.Messages)
                                    {
                                        Console.Write($"[{c}] ");
                                        WriteColor(err, ConsoleColor.Red);
                                        c++;
                                    }
                                }
                                if (trace != null)
                                {
                                    WriteColor("[STACK TRACE]:\n" + trace, ConsoleColor.Red);
                                }
                                if (_ex.InnerException != null)
                                {
                                    WriteColor(
                                        "[INNER MESSAGE]:\n" + _ex.InnerException.Message,
                                        ConsoleColor.Red
                                    );
                                }
                                if (dump.Success)
                                    WriteColor(
                                        $"[INFO] Crash dump saved at: '{dump.FilePath}'",
                                        ConsoleColor.Blue
                                    );
                            }
                        }
                    }
                }
                else if (!needsTUI) // was successful but from CLI
                {
                    if (CLIResult.Verbo != Settings.Verbosity.Quiet) // normal OR verbose
                    {
                        if (CLIResult.Verbo != Settings.Verbosity.Normal) // verbose
                            if (CLIResult.Messages != null && CLIResult.Messages.Count > 0)
                                foreach (string msg in CLIResult.Messages)
                                    WriteColor($"[INFO] " + msg, ConsoleColor.Blue);
                    }
                }
            }
            Environment.Exit((int)_ex.Code);
        }
    }
}
