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
                        if (oldFlags.extBlacklist.Any())
                        {
                            WriteMsg(
                                "Cannot edit the whitelist while filtering by 'blacklist'",
                                MsgType.Warning
                            );
                            WriteMsg(
                                "To clear a list, type in the keyword '--clear' instead of extensions",
                                MsgType.Info
                            );
                        }
                        else
                        {
                            string list = "CURRENT WHITELIST CONTENTS:";
                            foreach (string ext in oldFlags.extWhitelist)
                                list += $" {ext};";
                            WriteMsg(list, MsgType.Info);

                            oldFlags.extWhitelist = InputHandler.AskForExtensions(
                                "Please write the whitelisted extensions",
                                oldFlags.extWhitelist
                            );

                            list = "UPDATED WHITELIST CONTENTS:";
                            foreach (string ext in oldFlags.extWhitelist)
                                list += $" {ext};";
                            WriteMsg(list, MsgType.Success);
                        }
                        break;

                    case AdvancedOptions.Extensions_Blacklist:
                        Console.Clear();
                        if (oldFlags.extWhitelist.Any())
                        {
                            WriteMsg(
                                "Cannot edit the blacklist while filtering by 'whitelist'",
                                MsgType.Warning
                            );
                            WriteMsg(
                                "To clear a list, type in the keyword '--clear' instead of extensions",
                                MsgType.Info
                            );
                        }
                        else
                        {
                            string list = "CURRENT BLACKLIST CONTENTS:";
                            foreach (string ext in oldFlags.extBlacklist)
                                list += $" {ext};";
                            WriteMsg(list, MsgType.Info);
                            oldFlags.extBlacklist = InputHandler.AskForExtensions(
                                "Please write the blacklisted extensions",
                                oldFlags.extBlacklist
                            );
                            list = "UPDATED BLACKLIST CONTENTS:";
                            foreach (string ext in oldFlags.extBlacklist)
                                list += $" {ext};";
                            WriteMsg(list, MsgType.Success);
                        }
                        break;

                    case AdvancedOptions.Max_Search_Depth:
                        Console.Clear();
                        oldFlags.maxLevel = InputHandler.AskForInt(
                            "Maximum search depth",
                            1,
                            int.MaxValue
                        );
                        WriteMsg(
                            $"Maximum search depth was set to: '{oldFlags.maxLevel}'",
                            MsgType.Success
                        );
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
