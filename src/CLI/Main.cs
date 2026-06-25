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

            // target folder
            flags.targetDir = InputHandler.AskForDir();
            WriteMsg($"Directory chosen: '{flags.targetDir}'", MsgType.Success);

            // output format
            WriteMsg(
                "Choosing HTML for bigger folders is highly recommended.\n"
                    + "Opening other format reports for big folders will be a lot slower.",
                MsgType.Warning
            );
            flags.format =
                (
                    InputHandler.ChoiceMenu<OutputFormat>(
                        "Please choose the number of the preferred output format"
                    )
                ) ?? flags.format;

            WriteMsg($"Format chosen: '{flags.format}'", MsgType.Success);

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
                flags = AdvancedMenu.Edit(flags);
            }

            WriteMsg($"The report will be saved at: '{outputPath}'", MsgType.Info);
            WaitForInput();

            switch (flags.format)
            {
                case OutputFormat.HTML:
                    HtmlWriter hmtlWriter = new HtmlWriter(flags);
                    Task hmtlWrite = hmtlWriter.WriteAsync();
                    LoadingAnimation(hmtlWrite, "Generating report", 100);

                    await hmtlWrite;
                    if (hmtlWrite.IsCompletedSuccessfully)
                        ShowSuccess();
                    break;

                case OutputFormat.Markdown:
                    MarkdownWriter mdWriter = new MarkdownWriter(flags);
                    Task mdWrite = mdWriter.WriteAsync();
                    LoadingAnimation(mdWrite, "Generating report", 100);

                    await mdWrite;
                    if (mdWrite.IsCompletedSuccessfully)
                        ShowSuccess();
                    break;

                case OutputFormat.Text:
                    Core.Writers.TextWriter txtWriter = new Core.Writers.TextWriter(flags);
                    Task txtWrite = txtWriter.WriteAsync();
                    LoadingAnimation(txtWrite, "Generating report", 100);

                    await txtWrite;
                    if (txtWrite.IsCompletedSuccessfully)
                        ShowSuccess();
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

        private static void ShowSuccess() =>
            WriteMsg("REPORT GENERATED SUCCESSFULY", MsgType.Success);
    }
}
