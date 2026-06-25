using Core;
using static CLI.ColorDisplay;

namespace Launcher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length == 0)
            {
                CoreException? _ex = null;
                try
                {
                    await CLI.Main.Run();
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
                        WriteMsg($"[MESSAGE]: {_ex.Message}", MsgType.Info);
                    }
                    else
                    {
                        _ex = new(ErrorCode.Codes.Success, "REPORT GENERATED SUCCESSFULY");
                    }
                    WaitForInput();
                    Environment.Exit((int)_ex.Code);
                }
            }
            else
            {
                WriteMsg("'SILENT' IS IN DEVELOPMENT", MsgType.Warning);
            }
        }
    }
}
