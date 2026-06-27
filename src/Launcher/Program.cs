using Core;
using static CLI.ColorDisplay;

namespace Launcher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CoreException? _ex = null;
            try
            {
                if (args.Length == 0)
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
                _ex = ErrorCode.TranslateOSException(newEx);
            }
            finally
            {
                if (_ex != null)
                {
                    WriteMsg(
                        $"[{(int)_ex.Code}]: {ErrorCode.GetErrorMessage(_ex.Code)}",
                        MsgType.Error
                    );
                    if (_ex.Message != null)
                        WriteMsg($"[MESSAGE]: {_ex.Message}", MsgType.Info);
                }
                else
                {
                    _ex = new(ErrorCode.Codes.Success, "REPORT GENERATED SUCCESSFULY");
                }
                if (args.Length == 0)
                    WaitForInput();
                Environment.Exit((int)_ex.Code);
            }
        }
    }
}
