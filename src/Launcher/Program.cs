using Core;
using static CLI.ColorDisplay;
using static CLI.InputHandler;

namespace Launcher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CoreException? _ex = null;
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
                _ex = ExitCodes.TranslateOSException(newEx);
            }
            finally
            {
                if (_ex != null)
                {
                    if (needsCLI)
                    {
                        WriteMsg(
                            $"[{(int)_ex.Code}]: {ExitCodes.GetCodeMessage(_ex.Code)}",
                            MsgType.Error
                        );
                    }
                }
                else
                {
                    _ex = new(ExitCodes.Success, ExitCodes.GetCodeMessage(ExitCodes.Success));
                }
                if (needsCLI && _ex != null && (_ex.Code != ExitCodes.Success))
                {
                    bool errDetails = AskYN(
                        "Do you want to see detailed error information?",
                        false
                    );
                    if (errDetails)
                    {
                        WriteMsg("Detailed error information:", MsgType.Warning);
                        WriteMsg(Convert.ToString(_ex.Code), MsgType.Code);
                        WriteMsg($"[MESSAGE]: {_ex.Message}", MsgType.Error);
                    }
                }
                Environment.Exit((int)_ex.Code);
            }
        }
    }
}
