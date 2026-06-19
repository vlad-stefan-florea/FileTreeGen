using Core;
using static CLI.ColorDisplay;
using static Core.Settings;

namespace CLI
{
    internal class AdvancedMenu
    {
        public static GenFlags Edit(GenFlags oldFlags)
        {
            bool back = false;
            while (!back)
            {
                AdvancedOptions? option = InputHandler.ChoiceMenu<AdvancedOptions>(
                    "Choose an advanced option to edit"
                );
                if (option is null)
                {
                    back = true;
                    continue;
                }
                switch (option)
                {
                    case AdvancedOptions.Output_Directory:
                        Console.Clear();
                        string outputDir = InputHandler.AskForDir();
                        WriteMsg($"Directory chosen: '{outputDir}'", MsgType.Success);
                        oldFlags.outPath = Core.Utils.ReportInfo.GeneratePath(
                            outputDir,
                            oldFlags.targetDir,
                            oldFlags.format
                        );
                        break;

                    case AdvancedOptions.Buffer_Size:
                        Console.Clear();
                        oldFlags.bufferSize =
                            (
                                InputHandler.ChoiceMenu<BufferSize>(
                                    "Please choose the buffer size",
                                    cancelValue: oldFlags.bufferSize
                                )
                            ) ?? oldFlags.bufferSize;
                        WriteMsg(
                            $"Buffer Size was set to: '{oldFlags.bufferSize.ToString().Replace("_", " ")}'",
                            MsgType.Success
                        );
                        break;

                    case AdvancedOptions.Extensions_Whitelist:
                        Console.Clear();
                        break;

                    case AdvancedOptions.Extensions_Blacklist:
                        Console.Clear();
                        break;

                    default:
                        back = true;
                        break;
                }
            }
            return oldFlags; // I didn't want to create a copy, that's why i kept it as 'old' flags :3
        }
    }
}
