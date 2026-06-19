using Core;
using Core.Writers;
using static CLI.ColorDisplay;
using static Core.Settings;

namespace CLI
{
    public class Main
    {
        public static async Task Run()
        {
            // GenFlags
            GenFlags flags = new GenFlags();
            flags.noStatistics = true;
            flags.maxLevel = 2;

            // target folder
            flags.targetDir = InputHandler.AskForDir();
            WriteMsg($"Directory chosen: '{flags.targetDir}'", MsgType.Success);

            // output format
            WriteMsg(
                "Choosing HTML for bigger folders is highly recommended.\nOpening other format reports for big folders will be a lot slower.",
                MsgType.Warning
            );
            flags.format =
                (
                    InputHandler.ChoiceMenu<OutputFormat>(
                        "Please choose the number of the preferred output format"
                    )
                ) ?? flags.format;

            WriteMsg($"Format chosen: '{flags.format}'", MsgType.Success);

            if (flags.format == OutputFormat.HTML)
            {
                WriteMsg("IN DEVELOPMENT (SET TO TEXT)", MsgType.Warning);
                flags.format = OutputFormat.Text;
            }

            // DEFAULT VALUES
            string userprofile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                downloads = Path.Join(userprofile, "downloads"),
                outputPath = Core.Utils.ReportInfo.GeneratePath(
                    downloads,
                    flags.targetDir,
                    flags.format
                );
            flags.outPath = outputPath;

            // other settings prompt
            bool advanced = InputHandler.AskYN("Edit advanced settings?");
            if (advanced)
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
                            outputPath = Core.Utils.ReportInfo.GeneratePath(
                                outputDir,
                                flags.targetDir,
                                flags.format
                            );
                            flags.outPath = outputPath;
                            break;

                        case AdvancedOptions.Buffer_Size:
                            Console.Clear();
                            flags.bufferSize =
                                (
                                    InputHandler.ChoiceMenu<BufferSize>(
                                        "Please choose the buffer size",
                                        cancelValue: flags.bufferSize
                                    )
                                ) ?? flags.bufferSize;
                            WriteMsg(
                                $"Buffer Size was set to: '{flags.bufferSize.ToString().Replace("_", " ")}'",
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
            }

            WriteMsg($"The report will be saved at: '{outputPath}'", MsgType.Info);
            WaitForInput();

            switch (flags.format)
            {
                case OutputFormat.HTML:
                    WriteMsg("IN DEVELOPMENT", MsgType.Warning);
                    break;

                case OutputFormat.Markdown:
                    MarkdownWriter mdWriter = new MarkdownWriter(flags);
                    Task mdWrite = mdWriter.WriteAsync();
                    LoadingAnimation(mdWrite, "Generating report", 100);

                    await mdWrite;
                    if (mdWrite.IsCompletedSuccessfully)
                        WriteMsg("REPORT GENERATED", MsgType.Success);
                    else
                        WriteMsg("FAILED TO GENERATE REPORT", MsgType.Error);
                    break;

                case OutputFormat.Text:
                    Core.Writers.TextWriter txtWriter = new Core.Writers.TextWriter(flags);
                    Task txtWrite = txtWriter.WriteAsync();
                    LoadingAnimation(txtWrite, "Generating report", 100);

                    await txtWrite;
                    if (txtWrite.IsCompletedSuccessfully)
                        WriteMsg("REPORT GENERATED", MsgType.Success);
                    else
                        WriteMsg("FAILED TO GENERATE REPORT", MsgType.Error);
                    break;

                default:
                    break;
            }
            if (flags.autoOpenReport)
                Core.Utils.FileSystem.OpenPath(flags.outPath);
            else
            {
                bool ans = InputHandler.AskYN("Open report?");
                if (ans)
                    Core.Utils.FileSystem.OpenPath(flags.outPath);
            }
        }
    }
}
