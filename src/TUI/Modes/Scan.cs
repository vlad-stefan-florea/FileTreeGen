using Core;
using Core.Utils;
using Core.Writers;
using static Core.Settings;
using static TUI.Display;

namespace TUI.Modes
{
    internal class Scan
    {
        public static async Task Start()
        {
            // GenFlags
            GenFlags flags = new GenFlags
            {
                // target folder
                targetDir = InputHandler.AskForDir(false),
            };

            // output format
            flags.reportType =
                (InputHandler.ChoiceMenu<ReportType>("Please choose the report's type:"))
                ?? flags.reportType;

            // other settings prompt
            bool advanced = InputHandler.AskYN("Edit advanced settings?", false);
            if (advanced)
            {
                flags = AdvancedMenu.Edit(flags);
            }

            string outPath = flags.GetOutPath();

            WriteMsg($"The report will be saved at: '{outPath}'", MsgType.Info);
            WaitForInput();

            switch (flags.reportType)
            {
                case ReportType.HTML:
                    HtmlWriter hmtlWriter = new HtmlWriter(flags);
                    Task hmtlWrite = hmtlWriter.WriteAsync();
                    LoadingAnimation(hmtlWrite, "Generating report", 100, true);

                    await hmtlWrite;
                    if (hmtlWrite.IsCompletedSuccessfully)
                        ShowSuccess();
                    break;

                case ReportType.Markdown:
                    MarkdownWriter mdWriter = new MarkdownWriter(flags);
                    Task mdWrite = mdWriter.WriteAsync();
                    LoadingAnimation(mdWrite, "Generating report", 100, true);

                    await mdWrite;
                    if (mdWrite.IsCompletedSuccessfully)
                        ShowSuccess();
                    break;

                case ReportType.Text:
                    Core.Writers.TextWriter txtWriter = new Core.Writers.TextWriter(flags);
                    Task txtWrite = txtWriter.WriteAsync();
                    LoadingAnimation(txtWrite, "Generating report", 100, true);

                    await txtWrite;
                    if (txtWrite.IsCompletedSuccessfully)
                        ShowSuccess();
                    break;

                default:
                    break;
            }
            if (flags.autoOpenReport)
                FileSystem.OpenPath(outPath);
            else
            {
                bool ans = InputHandler.AskYN("Open the report?", true);
                if (ans)
                    FileSystem.OpenPath(outPath);
            }
        }

        private static void ShowSuccess() =>
            WriteMsg("REPORT GENERATED SUCCESSFULY", MsgType.Success);
    }
}
