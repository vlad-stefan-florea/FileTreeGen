using Core;
using static CLI.Display;
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
                    {
                        oldFlags.autoOpenReport = InputHandler.AskForSwitch(
                            "Automatically open the report after generation?",
                            oldFlags.autoOpenReport
                        );
                        break;
                    }

                    case CliAdvancedOptions.Buffer_Size:
                    {
                        oldFlags.bufferSize =
                            (InputHandler.ChoiceMenu<BufferSize>("Please choose the buffer size:"))
                            ?? oldFlags.bufferSize;
                        break;
                    }

                    case CliAdvancedOptions.Directories_Only:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Include ONLY FOLDERS in the generated report?",
                            oldFlags.dirsOnly
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.dirsOnly = newValue;
                        });
                        if (!change.IsValid)
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            break;
                        }
                        oldFlags.dirsOnly = newValue;
                        break;
                    }

                    case CliAdvancedOptions.Extensions_Blacklist:
                    {
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.extBlacklist = ["<placeholder>"]; // simulate non-empty blacklist
                        });
                        if (!change.IsValid)
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            break;
                        }

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
                        break;
                    }

                    case CliAdvancedOptions.Extensions_Whitelist:
                    {
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.extWhitelist = ["<placeholder>"]; // simulate non-empty whitelist
                        });
                        if (!change.IsValid)
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            break;
                        }

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
                            list += string.IsNullOrEmpty(ext) ? " \"\";" : $" {ext};";
                        WriteMsg(list, MsgType.Save);
                        break;
                    }

                    case CliAdvancedOptions.Files_Only:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Include ONLY FILES in the generated report?",
                            oldFlags.filesOnly
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.filesOnly = newValue;
                        });
                        if (!change.IsValid)
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            break;
                        }
                        oldFlags.filesOnly = newValue;
                        break;
                    }

                    case CliAdvancedOptions.Ignore_Empty_Directories:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Exclude all empty folders from the report?",
                            oldFlags.ignoreEmptyDirs
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.ignoreEmptyDirs = newValue;
                        });
                        if (!change.IsValid)
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            break;
                        }
                        oldFlags.ignoreEmptyDirs = newValue;
                        break;
                    }

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
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Include icons in the generated report?",
                            oldFlags.includeIcons
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.includeIcons = newValue;
                        });
                        if (!change.IsValid)
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            break;
                        }
                        oldFlags.includeIcons = newValue;
                        break;
                    }

                    case CliAdvancedOptions.Format_Report:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "If the report should be formatted based on the output type or just a plain text list.",
                            oldFlags.formatReport
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.includeIcons = newValue;
                        });
                        if (!change.IsValid)
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            break;
                        }
                        oldFlags.formatReport = newValue;
                        if (!newValue)
                        {
                            oldFlags.includeIcons = false;
                            WriteMsg(
                                "The 'Include Icons' option was turned off automatically.",
                                MsgType.Warning
                            );
                        }
                        break;
                    }

                    case CliAdvancedOptions.Include_Statistics:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Include statistics in the generated report?",
                            oldFlags.includeStatistics
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.includeStatistics = newValue;
                        });
                        if (!change.IsValid)
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            break;
                        }
                        oldFlags.includeStatistics = newValue;
                        break;
                    }

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
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Include just the tree in the generated report?",
                            oldFlags.treeOnly
                        );
                        if (oldFlags.includeStatistics)
                        {
                            oldFlags.includeStatistics = false;
                            WriteMsg(
                                "The 'Include Statistics' option was turned off automatically.",
                                MsgType.Warning
                            );
                        }
                        oldFlags.treeOnly = newValue;
                        break;
                    }

                    case CliAdvancedOptions.Ignore_SymLinks:
                        oldFlags.ignoreSymlinks = InputHandler.AskForSwitch(
                            "Exclude all symlinks from the report?",
                            oldFlags.ignoreSymlinks
                        );
                        break;

                    default:
                        back = true;
                        break;
                }
            }
            return oldFlags; // I didn't want to create a copy, that's why i kept it as 'old' flags :3

            // flags validation
            (bool IsValid, string Message) TryApplyChange(Action<GenFlags> Change)
            {
                bool IsValid = true;
                string Message = string.Empty;
                GenFlags Candidate = new(oldFlags);
                Change(Candidate);

                bool onlyFiles = Candidate.filesOnly,
                    onlyDirs = Candidate.dirsOnly,
                    ignoreEmptyDirs = Candidate.ignoreEmptyDirs,
                    byWhitelist = Candidate.extWhitelist.Count > 0,
                    byBlacklist = Candidate.extBlacklist.Count > 0,
                    dontFormat = !Candidate.formatReport,
                    treeOnly = Candidate.treeOnly,
                    includeStats = Candidate.includeStatistics,
                    useIcons = Candidate.includeIcons;

                if (onlyFiles && onlyDirs)
                {
                    IsValid = false;
                    Message =
                        "The options 'Directories Only' and 'Files Only' cannot be used simultaneously.";
                }
                if (byWhitelist && byBlacklist)
                {
                    IsValid = false;
                    Message = "Cannot filter by 'Whitelist' and 'Blacklist' simultaneously.";
                }
                if (byWhitelist && onlyDirs)
                {
                    IsValid = false;
                    Message = "Cannot filter by Whitelist while 'Directories Only' is active.";
                }
                if (byBlacklist && onlyDirs)
                {
                    IsValid = false;
                    Message = "Cannot filter by Blacklist while 'Directories Only' is active.";
                }
                if (onlyFiles && ignoreEmptyDirs)
                {
                    IsValid = false;
                    Message =
                        "Cannot activate the option 'Ignore Empty Directories' while 'files only' is active.";
                }
                if (Candidate.reportType == ReportType.HTML && dontFormat) // not really an issue, mostly for info
                {
                    IsValid = true;
                    Message = "The 'Format Report' option has no effect on HTML reports.";
                }
                if (dontFormat && useIcons)
                {
                    IsValid = false;
                    Message = "Cannot include icons while 'No Report Formatting' is active.";
                }
                if (treeOnly && includeStats)
                {
                    IsValid = false;
                    Message =
                        "Cannot include statistics in the report while 'Tree Only' is active.";
                }
                return (IsValid, Message);
            }
        }
    }
}
