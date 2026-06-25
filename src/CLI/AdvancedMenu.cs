using Core;
using static CLI.ColorDisplay;
using static Core.Settings;

namespace CLI
{
    internal class AdvancedMenu
    {
        public static GenFlags Edit(GenFlags oldFlags)
        {
            int maxPrompts = 10,
                currentPrompt = 0;
            bool back = false;
            Console.Clear();
            while (!back)
            {
                currentPrompt++;
                if (currentPrompt % maxPrompts == 0)
                    Console.Clear();
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
                    case AdvancedOptions.Auto_Open_Report:
                        oldFlags.autoOpenReport = InputHandler.AskForSwitch(
                            "Automatically open the report after generation",
                            oldFlags.autoOpenReport
                        );
                        break;

                    case AdvancedOptions.Buffer_Size:
                        oldFlags.bufferSize =
                            (InputHandler.ChoiceMenu<BufferSize>("Please choose the buffer size"))
                            ?? oldFlags.bufferSize;
                        break;

                    case AdvancedOptions.Directories_Only:
                        oldFlags.dirsOnly = InputHandler.AskForSwitch(
                            "Include ONLY FOLDERS in the generated report",
                            oldFlags.dirsOnly
                        );
                        break;

                    case AdvancedOptions.Extensions_Blacklist:
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
                                list += string.IsNullOrEmpty(ext) ? "\"\"" : $" {ext};";
                            WriteMsg(list, MsgType.Success);
                        }
                        break;

                    case AdvancedOptions.Extensions_Whitelist:
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
                            foreach (string ext in oldFlags.extBlacklist)
                                list += string.IsNullOrEmpty(ext) ? "\"\"" : $" {ext};";
                            WriteMsg(list, MsgType.Success);
                        }
                        break;

                    case AdvancedOptions.Files_Only:
                        oldFlags.filesOnly = InputHandler.AskForSwitch(
                            "Include ONLY FILES in the generated report",
                            oldFlags.filesOnly
                        );
                        break;

                    case AdvancedOptions.Ignore_Empty_Directories:
                        oldFlags.ignoreEmptyDirs = InputHandler.AskForSwitch(
                            "Exclude all empty folders from the report",
                            oldFlags.ignoreEmptyDirs
                        );
                        break;

                    case AdvancedOptions.Max_Search_Depth:
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

                    case AdvancedOptions.No_Icons:
                        oldFlags.noIcons = InputHandler.AskForSwitch(
                            "Exclude all icons from the generated report",
                            oldFlags.noIcons
                        );
                        break;

                    case AdvancedOptions.No_Report_Formatting:
                        if (oldFlags.format == Settings.OutputFormat.HTML)
                        {
                            WriteMsg(
                                "The 'No Report Formatting' setting cannot be applied while the output format is set to 'HTML'",
                                MsgType.Warning
                            );
                            break;
                        }
                        oldFlags.noFormatting = InputHandler.AskForSwitch(
                            "Generate the report as a plain text list without formatting",
                            oldFlags.noFormatting
                        );
                        break;

                    case AdvancedOptions.No_Statistics:
                        oldFlags.noStatistics = InputHandler.AskForSwitch(
                            "Disable all statistics calculations (results in faster generation)",
                            oldFlags.noStatistics
                        );
                        break;

                    case AdvancedOptions.Output_Directory:
                        string outputDir = InputHandler.AskForDir();
                        oldFlags.outPath = Core.Utils.ReportInfo.GeneratePath(
                            outputDir,
                            oldFlags.targetDir,
                            oldFlags.format
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
