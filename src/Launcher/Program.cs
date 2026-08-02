using Core;
using static CLI.Display;
using static CLI.InputHandler;

namespace Launcher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CoreException _ex = new(ExitCode.Success, ExitMessages.Get(ExitCode.Success));
            bool needsCLI = args.Length == 0;
            try
            {
                if (needsCLI)
                {
                    await CLI.Main.Run();
                }
                else
                {
                    WriteMsg("This feature is currently in development 🛠️", MsgType.Warning);
                    //await Silent.Main.ParseAndRun(args);
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
                if (needsCLI)
                {
                    if (_ex.Code != ExitCode.Success)
                    {
                        WriteMsg($"[{_ex.Code}]: {_ex.Message}", MsgType.Error);
                        bool errDetails = AskYN(
                            "Do you want to see the detailed error information?",
                            false
                        );
                        if (errDetails)
                        {
                            WriteMsg("[CODE]: " + _ex.Code, MsgType.Error);
                            WriteMsg("[DESCRIPTION]: " + _ex.Code, MsgType.Error);
                            if (_ex.InnerException != null)
                            {
                                WriteMsg(
                                    "[INNER MESSAGE]:\n" + _ex.InnerException.Message,
                                    MsgType.Error
                                );
                                WriteMsg(
                                    "[INNER STACK TRACE]:\n" + _ex.InnerException.StackTrace,
                                    MsgType.Error
                                );
                            }
                        }
                    }
                    else { }
                }

                Environment.Exit((int)_ex.Code);
            }
        }
    }
}
