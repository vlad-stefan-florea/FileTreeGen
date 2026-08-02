using Core;
using Core.Writers;
using static CLI.Display;
using static Core.Settings;

namespace CLI
{
    public class Main
    {
        private static string Title = """
                _______ __   ______               ______         
               / ____(_) /__/_  __/_______  ___  / ____/__  ____ 
              / /_  / / / _ \/ / / ___/ _ \/ _ \/ / __/ _ \/ __ \
             / __/ / / /  __/ / / /  /  __/  __/ /_/ /  __/ / / /
            /_/   /_/_/\___/_/ /_/   \___/\___/\____/\___/_/ /_/ 
            """;

        private static string AppHeaderInfo =
            "Version: " + AppInfo.Version + " | Developed by: " + AppInfo.Developer;

        public static async Task Run()
        {
            WriteColor(Title, ConsoleColor.Green);
            WriteColor(AppHeaderInfo, ConsoleColor.DarkGreen);
            // GenFlags
            GenFlags flags = new GenFlags();

            // target folder
            flags.targetDir = InputHandler.AskForDir();

            // output format
            flags.reportType =
                (InputHandler.ChoiceMenu<OutputFormat>("Please choose the report's type:"))
                ?? flags.reportType;

            // DEFAULT VALUES
            string userprofile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                downloads = Path.Join(userprofile, "downloads"),
                outputPath = Core.Utils.ReportInfo.GeneratePath(
                    downloads,
                    flags.targetDir,
                    flags.reportType
                );
            flags.outPath = outputPath;

            // other settings prompt
            bool advanced = InputHandler.AskYN("Edit advanced settings?", false);
            if (advanced)
            {
                flags = AdvancedMenu.Edit(flags);
            }

            WriteMsg($"The report will be saved at: '{outputPath}'", MsgType.Info);
            WaitForInput();

            switch (flags.reportType)
            {
                case OutputFormat.HTML:
                    HtmlWriter hmtlWriter = new HtmlWriter(flags);
                    Task hmtlWrite = hmtlWriter.WriteAsync();
                    LoadingAnimation(hmtlWrite, "Generating report", 100, true);

                    await hmtlWrite;
                    if (hmtlWrite.IsCompletedSuccessfully)
                        ShowSuccess();
                    break;

                case OutputFormat.Markdown:
                    MarkdownWriter mdWriter = new MarkdownWriter(flags);
                    Task mdWrite = mdWriter.WriteAsync();
                    LoadingAnimation(mdWrite, "Generating report", 100, true);

                    await mdWrite;
                    if (mdWrite.IsCompletedSuccessfully)
                        ShowSuccess();
                    break;

                case OutputFormat.Text:
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
                Core.Utils.FileSystem.OpenPath(flags.outPath);
            else
            {
                bool ans = InputHandler.AskYN("Open the report?", true);
                if (ans)
                    Core.Utils.FileSystem.OpenPath(flags.outPath);
            }
        }

        private static void ShowSuccess() =>
            WriteMsg("REPORT GENERATED SUCCESSFULY", MsgType.Success);
    }
}
