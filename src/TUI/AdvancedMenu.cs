using Core;
using Core.Utils;
using static Core.Settings;
using static TUI.Display;

namespace TUI
{
    internal class AdvancedMenu
    {
        public static GenFlags Edit(GenFlags oldFlags)
        {
            bool back = false;
            while (!back)
            {
                TuiAdvancedOptions? option = InputHandler.ChoiceMenu<TuiAdvancedOptions>(
                    "Choose an advanced option to edit"
                );
                if (option is null)
                {
                    back = true;
                    continue;
                }

                switch (option)
                {
                    #region REPORT
                    case TuiAdvancedOptions.Report_Name_Scheme:
                        oldFlags.reportNameScheme =
                            (
                                InputHandler.ChoiceMenu<ReportNameScheme>(
                                    "Please choose the report's naming scheme:"
                                )
                            ) ?? oldFlags.reportNameScheme;
                        break;
                    case TuiAdvancedOptions.Tree_Only:
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
                    #endregion

                    #region FILTERING
                    case TuiAdvancedOptions.Directories_Only:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Include ONLY FOLDERS in the generated report?",
                            oldFlags.dirsOnly
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.dirsOnly = newValue;
                        });
                        if (!string.IsNullOrEmpty(change.Message))
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            if (!change.IsValid)
                                break;
                        }
                        oldFlags.dirsOnly = newValue;
                        break;
                    }
                    case TuiAdvancedOptions.Director_Names_Blacklist:
                    {
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.dirBlacklist = ["<placeholder>"]; // simulate non-empty blacklist
                        });
                        if (!string.IsNullOrEmpty(change.Message))
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            if (!change.IsValid)
                                break;
                        }

                        string list = "CURRENTLY EXCLUDED FOLDERS:";
                        foreach (string ext in oldFlags.dirBlacklist)
                            list += $" {ext};";
                        WriteMsg(list, MsgType.Info);
                        oldFlags.dirBlacklist = InputHandler.AskForHashSet(
                            "Please write the blacklisted directory names",
                            oldFlags.dirBlacklist
                        );
                        list = "UPDATED BLACKLIST CONTENTS:";
                        foreach (string ext in oldFlags.dirBlacklist)
                            list += $" {ext};";
                        WriteMsg(list, MsgType.Save);
                        break;
                    }
                    case TuiAdvancedOptions.Extensions_Blacklist:
                    {
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.extBlacklist = ["<placeholder>"]; // simulate non-empty blacklist
                        });
                        if (!string.IsNullOrEmpty(change.Message))
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            if (!change.IsValid)
                                break;
                        }

                        string list = "CURRENT BLACKLIST CONTENTS:";
                        foreach (string ext in oldFlags.extBlacklist)
                            list += $" {ext};";
                        WriteMsg(list, MsgType.Info);
                        oldFlags.extBlacklist = HashSetParser.ToExtHashSet(
                            InputHandler.AskForHashSet(
                                "Please write the blacklisted extensions",
                                oldFlags.extBlacklist
                            )
                        );
                        list = "UPDATED BLACKLIST CONTENTS:";
                        foreach (string ext in oldFlags.extBlacklist)
                            list += string.IsNullOrEmpty(ext) ? " \"\";" : $" {ext};";
                        WriteMsg(list, MsgType.Save);
                        break;
                    }
                    case TuiAdvancedOptions.Extensions_Whitelist:
                    {
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.extWhitelist = ["<placeholder>"]; // simulate non-empty whitelist
                        });
                        if (!string.IsNullOrEmpty(change.Message))
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            if (!change.IsValid)
                                break;
                        }

                        string list = "CURRENT WHITELIST CONTENTS:";
                        foreach (string ext in oldFlags.extWhitelist)
                            list += $" {ext};";
                        WriteMsg(list, MsgType.Info);

                        oldFlags.extWhitelist = HashSetParser.ToExtHashSet(
                            InputHandler.AskForHashSet(
                                "Please write the whitelisted extensions",
                                oldFlags.extWhitelist
                            )
                        );
                        list = "UPDATED WHITELIST CONTENTS:";
                        foreach (string ext in oldFlags.extWhitelist)
                            list += string.IsNullOrEmpty(ext) ? " \"\";" : $" {ext};";
                        WriteMsg(list, MsgType.Save);
                        break;
                    }
                    case TuiAdvancedOptions.Files_Only:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Include ONLY FILES in the generated report?",
                            oldFlags.filesOnly
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.filesOnly = newValue;
                        });
                        if (!string.IsNullOrEmpty(change.Message))
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            if (!change.IsValid)
                                break;
                        }
                        oldFlags.filesOnly = newValue;
                        break;
                    }
                    #endregion

                    #region SCAN
                    case TuiAdvancedOptions.Auto_Open_Report:
                    {
                        oldFlags.autoOpenReport = InputHandler.AskForSwitch(
                            "Automatically open the report after generation?",
                            oldFlags.autoOpenReport
                        );
                        break;
                    }
                    case TuiAdvancedOptions.Buffer_Size:
                    {
                        oldFlags.bufferSize =
                            (InputHandler.ChoiceMenu<BufferSize>("Please choose the buffer size:"))
                            ?? oldFlags.bufferSize;
                        break;
                    }
                    case TuiAdvancedOptions.Ignore_Empty_Directories:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Exclude all empty folders from the report?",
                            oldFlags.ignoreEmptyDirs
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.ignoreEmptyDirs = newValue;
                        });
                        if (!string.IsNullOrEmpty(change.Message))
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            if (!change.IsValid)
                                break;
                        }
                        oldFlags.ignoreEmptyDirs = newValue;
                        break;
                    }
                    case TuiAdvancedOptions.Ignore_SymLinks:
                        oldFlags.ignoreSymlinks = InputHandler.AskForSwitch(
                            "Exclude all symlinks from the report?",
                            oldFlags.ignoreSymlinks
                        );
                        break;
                    case TuiAdvancedOptions.Include_Statistics:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Include statistics in the generated report?",
                            oldFlags.includeStatistics
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.includeStatistics = newValue;
                        });
                        if (!string.IsNullOrEmpty(change.Message))
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            if (!change.IsValid)
                                break;
                        }
                        oldFlags.includeStatistics = newValue;
                        break;
                    }
                    case TuiAdvancedOptions.Max_Search_Depth:
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
                    #endregion

                    #region PRESEENTATION
                    case TuiAdvancedOptions.Include_Icons:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "Include icons in the generated report?",
                            oldFlags.includeIcons
                        );
                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.includeIcons = newValue;
                        });
                        if (!string.IsNullOrEmpty(change.Message))
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            if (!change.IsValid)
                                break;
                        }
                        oldFlags.includeIcons = newValue;
                        break;
                    }
                    case TuiAdvancedOptions.Format_Report:
                    {
                        bool newValue = InputHandler.AskForSwitch(
                            "If the report should be formatted based on the output type or just a plain text list.",
                            oldFlags.formatReport
                        );
                        if (oldFlags.reportType == ReportType.HTML)
                            WriteMsg(
                                "The 'Format Report' option has no effect on HTML reports.",
                                MsgType.Info
                            );

                        var change = TryApplyChange(oldFlags =>
                        {
                            oldFlags.includeIcons = newValue;
                        });
                        if (!string.IsNullOrEmpty(change.Message))
                        {
                            WriteMsg(change.Message, MsgType.Warning);
                            if (!change.IsValid)
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
                    case TuiAdvancedOptions.Node_Label_Scheme:
                        oldFlags.nodeLabel =
                            (
                                InputHandler.ChoiceMenu<NodeLabel>(
                                    "Please choose the node label (naming scheme):"
                                )
                            ) ?? oldFlags.nodeLabel;
                        break;
                    #endregion

                    #region OUTPUT
                    case TuiAdvancedOptions.Output_Directory:
                        oldFlags.outDir = InputHandler.AskForDir();
                        break;
                    #endregion

                    default:
                        back = true;
                        break;
                }
            }
            return oldFlags;

            // flags validation
            (bool IsValid, string Message) TryApplyChange(Action<GenFlags> Change)
            {
                bool IsValid = false;
                string Message = string.Empty;
                GenFlags Candidate = new(oldFlags);
                Change(Candidate);

                bool onlyFiles = Candidate.filesOnly,
                    onlyDirs = Candidate.dirsOnly,
                    ignoreEmptyDirs = Candidate.ignoreEmptyDirs,
                    ignoreSymlinks = Candidate.ignoreSymlinks,
                    byWhitelist = Candidate.extWhitelist.Count > 0,
                    byBlacklist = Candidate.extBlacklist.Count > 0,
                    excludeDirs = Candidate.dirBlacklist.Count > 0,
                    dontFormat = !Candidate.formatReport,
                    treeOnly = Candidate.treeOnly,
                    includeStats = Candidate.includeStatistics,
                    useIcons = Candidate.includeIcons;

                if (onlyFiles && onlyDirs)
                {
                    Message =
                        "The options 'Directories Only' and 'Files Only' cannot be used simultaneously.";
                }
                if (byWhitelist && byBlacklist)
                {
                    Message = "Cannot filter by 'Whitelist' and 'Blacklist' simultaneously.";
                }
                if (byWhitelist && onlyDirs)
                {
                    Message = "Cannot filter by Whitelist while 'Directories Only' is active.";
                }
                if (byBlacklist && onlyDirs)
                {
                    Message = "Cannot filter by Blacklist while 'Directories Only' is active.";
                }
                if (onlyFiles && ignoreEmptyDirs)
                {
                    Message =
                        "Cannot activate the option 'Ignore Empty Directories' while 'Files Only' is active.";
                }
                if (onlyFiles && excludeDirs)
                {
                    Message = "Cannot exclude directories while 'Files Only' is active.";
                }
                if (dontFormat && useIcons)
                {
                    Message = "Cannot include icons while 'No Report Formatting' is active.";
                }
                if (treeOnly && includeStats)
                {
                    Message =
                        "Cannot include statistics in the report while 'Tree Only' is active.";
                }
                IsValid = string.IsNullOrEmpty(Message);
                return (IsValid, Message);
            }
        }

        public enum TuiAdvancedOptions
        {
            Auto_Open_Report,
            Buffer_Size,
            Directories_Only,
            Director_Names_Blacklist,
            Extensions_Blacklist,
            Extensions_Whitelist,
            Files_Only,
            Format_Report,
            Ignore_Empty_Directories,
            Ignore_SymLinks,
            Include_Icons,
            Include_Statistics,
            Max_Search_Depth,
            Node_Label_Scheme,
            Output_Directory,
            Report_Name_Scheme,
            Tree_Only,
        }
    }
}
