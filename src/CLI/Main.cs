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
            // target folder
            string targetDir = InputHandler.AskForDir();
            WriteMsg($"Directory chosen: '{targetDir}'", MsgType.Success);

            // output format
            OutputFormat format = InputHandler.ChoiceMenu<OutputFormat>(
                "Please choose the number of the preferred output format"
            );
            WriteMsg($"Format chosen: '{format}'", MsgType.Success);
            if (format == OutputFormat.HTML || format == OutputFormat.Markdown)
                WriteMsg(
                    "IN DEVELOPMENT: TEXT will be automatically set as the default output format",
                    MsgType.Warning
                );
            format = OutputFormat.Text;

            // DEFAULT VALUES
            string userprofile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                downloads = Path.Join(userprofile, "downloads"),
                outputPath = Core.Utils.ReportInfo.GeneratePath(downloads, targetDir, format);
            BufferSize bufferSize = BufferSize.Medium;
            List<string> ExtWhitelist = new(),
                ExtBlacklist = new();

            // other settings prompt
            bool advanced = InputHandler.AskYN("Edit advanced settings?");
            if (advanced)
            {
                bool back = false;
                while (!back)
                {
                    AdvancedOptions option = InputHandler.ChoiceMenu<AdvancedOptions>(
                        "Choose an advanced option to edit"
                    );
                    switch (option)
                    {
                        case AdvancedOptions.Back:
                            back = true;
                            break;

                        case AdvancedOptions.Output_Directory:
                            Console.Clear();
                            string outputDir = InputHandler.AskForDir();
                            WriteMsg($"Directory chosen: '{outputDir}'", MsgType.Success);
                            outputPath = Core.Utils.ReportInfo.GeneratePath(
                                outputDir,
                                targetDir,
                                format
                            );
                            break;

                        case AdvancedOptions.Buffer_Size:
                            Console.Clear();
                            bufferSize = InputHandler.ChoiceMenu<BufferSize>(
                                "Please choose the buffer size"
                            );
                            WriteMsg($"Buffer Size chosen: '{bufferSize}'", MsgType.Success);
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

            switch (format)
            {
                case OutputFormat.HTML:
                    WriteMsg("IN DEVELOPMENT", MsgType.Warning);
                    break;
                case OutputFormat.Markdown:
                    WriteMsg("IN DEVELOPMENT", MsgType.Warning);
                    break;
                case OutputFormat.Text:
                    TxtWriter writer = new TxtWriter(targetDir, outputPath, bufferSize);
                    Task write = writer.WriteAsync();

                    LoadingAnimation(write, 500);
                    Console.WriteLine();

                    await write;
                    WriteMsg("REPORT GENERATED", MsgType.Success);
                    break;
                default:
                    break;
            }
        }
    }
}
