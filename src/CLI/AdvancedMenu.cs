using Core;
using Core.Utils;
using static CLI.Display;
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
            while (!back)
            {
                currentPrompt++;
                if (currentPrompt % maxPrompts == 0)
                    Console.Clear();
                CliAdvancedOptions? option = InputHandler.ChoiceMenu<CliAdvancedOptions>(
                    "Choose an advanced option to edit"
                );
                if (option is null)
                {
                    back = true;
                    continue;
                }
                switch (option)
                {
                    case CliAdvancedOptions.Auto_Open_Report:
                        oldFlags.autoOpenReport = InputHandler.AskForSwitch(
                            "Automatically open the report after generation?",
                            oldFlags.autoOpenReport
                        );
                        break;

                    case CliAdvancedOptions.Buffer_Size:
                        oldFlags.bufferSize =
                            (InputHandler.ChoiceMenu<BufferSize>("Please choose the buffer size:"))
                            ?? oldFlags.bufferSize;
                        break;

                    case CliAdvancedOptions.Directories_Only:
                        if (oldFlags.filesOnly)
                        {
                            WriteMsg(
                                "Cannot activate the option 'directories only' while 'files only' is active.",
                                MsgType.Warning
                            );
                        }
                        else
                            oldFlags.dirsOnly = InputHandler.AskForSwitch(
                                "Include ONLY FOLDERS in the generated report?",
                                oldFlags.dirsOnly
                            );
                        break;

                    case CliAdvancedOptions.Extensions_Blacklist:
                        if (oldFlags.dirsOnly)
                        {
                            WriteMsg(
                                "Cannot edit the blacklist while 'directories only' is active.",
                                MsgType.Warning
                            );
                        }
                        else if (oldFlags.extWhitelist.Any())
                        {
                            WriteMsg(
                                "Cannot edit the blacklist while filtering by 'whitelist'.",
                                MsgType.Warning
                            );
                            WriteMsg(
                                "To clear a list, type in the keyword '--clear' instead of extensions.",
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
                                list += string.IsNullOrEmpty(ext) ? " \"\";" : $" {ext};";
                            WriteMsg(list, MsgType.Save);
                        }
                        break;

                    case CliAdvancedOptions.Extensions_Whitelist:
                        if (oldFlags.dirsOnly)
                        {
                            WriteMsg(
                                "Cannot edit the whitelist while 'directories only' is active.",
                                MsgType.Warning
                            );
                        }
                        else if (oldFlags.extBlacklist.Any())
                        {
                            WriteMsg(
                                "Cannot edit the whitelist while filtering by 'blacklist'.",
                                MsgType.Warning
                            );
                            WriteMsg(
                                "To clear a list, type in the keyword '--clear' instead of extensions.",
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
                                list += string.IsNullOrEmpty(ext) ? " \"\";" : $" {ext};";
                            WriteMsg(list, MsgType.Save);
                        }
                        break;

                    case CliAdvancedOptions.Files_Only:
                        if (oldFlags.filesOnly)
                        {
                            WriteMsg(
                                "Cannot activate the option 'files only' while 'directories only' is active.",
                                MsgType.Warning
                            );
                        }
                        else
                            oldFlags.filesOnly = InputHandler.AskForSwitch(
                                "Include ONLY FILES in the generated report?",
                                oldFlags.filesOnly
                            );
                        break;

                    case CliAdvancedOptions.Ignore_Empty_Directories:
                        if (oldFlags.filesOnly)
                        {
                            WriteMsg(
                                "Cannot activate the option 'ignore empty directories' while 'files only' is active.",
                                MsgType.Warning
                            );
                        }
                        else
                            oldFlags.ignoreEmptyDirs = InputHandler.AskForSwitch(
                                "Exclude all empty folders from the report?",
                                oldFlags.ignoreEmptyDirs
                            );
                        break;

                    case CliAdvancedOptions.Max_Search_Depth:
                        oldFlags.maxDepth = InputHandler.AskForInt(
                            "Maximum search depth",
                            1,
                            int.MaxValue
                        );
                        WriteMsg(
                            $"Maximum search depth was set to: '{oldFlags.maxDepth}'",
                            MsgType.Save
                        );
                        break;

                    case CliAdvancedOptions.Include_Icons:
                        oldFlags.includeIcons = InputHandler.AskForSwitch(
                            "Include icons in the generated report?",
                            oldFlags.includeIcons
                        );
                        break;

                    case CliAdvancedOptions.Format_Report:
                        oldFlags.formatReport = InputHandler.AskForSwitch(
                            "If the report should be formatted based on the output type or just a plain text list.",
                            oldFlags.formatReport
                        );
                        break;

                    case CliAdvancedOptions.Include_Statistics:
                        if (oldFlags.treeOnly)
                        {
                            WriteMsg(
                                "Cannot include statistics in the report when 'tree only' is active.",
                                MsgType.Warning
                            );
                        }
                        else
                            oldFlags.includeStatistics = InputHandler.AskForSwitch(
                                "Include statistics in the generated report?",
                                oldFlags.includeStatistics
                            );
                        break;

                    case CliAdvancedOptions.Output_Directory:
                        oldFlags.outDir = InputHandler.AskForDir();
                        break;

                    case CliAdvancedOptions.Node_Label_Scheme:
                        oldFlags.nodeLabel =
                            (
                                InputHandler.ChoiceMenu<NodeLabel>(
                                    "Please choose the node label (naming scheme):"
                                )
                            ) ?? oldFlags.nodeLabel;
                        break;

                    case CliAdvancedOptions.Report_Name_Scheme:
                        oldFlags.reportNameScheme =
                            (
                                InputHandler.ChoiceMenu<ReportNameScheme>(
                                    "Please choose the report's naming scheme:"
                                )
                            ) ?? oldFlags.reportNameScheme;
                        break;

                    case CliAdvancedOptions.Tree_Only:
                        oldFlags.treeOnly = InputHandler.AskForSwitch(
                            "Include statistics in the generated report?",
                            oldFlags.treeOnly
                        );
                        if (oldFlags.treeOnly && oldFlags.includeStatistics)
                        {
                            oldFlags.includeStatistics = false;
                            WriteMsg(
                                "The 'include statistics' is incompatible with 'tree only' and has been turned off.",
                                MsgType.Warning
                            );
                        }
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
