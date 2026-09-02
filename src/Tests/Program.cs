using System.Diagnostics;
using static TUI.Display;

namespace Tests
{
    internal class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string targetDir = Path.GetFullPath(".");
            string outDir = Path.Combine(Path.GetTempPath(), "ftg_tests");
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            var scanTests = TestsData.ScanCmd;
            DrawDefaults();
            int c = 1;
            bool success = true;

            // build the Launcher
            WriteMsg($"Build started for Launcher.csproj", MsgType.Info);
            var buildPsi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = @"build ..\..\..\..\Launcher\Launcher.csproj",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            using (var buildProcess = Process.Start(buildPsi)!)
            {
                await buildProcess.WaitForExitAsync();
                if (buildProcess.ExitCode != 0)
                {
                    string buildError = await buildProcess.StandardError.ReadToEndAsync();
                    WriteMsg($"Build Failed:\n{buildError}", MsgType.Error);
                    return;
                }
            }
            WriteMsg($"Build succeeded!", MsgType.Success);

            // start the tests
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var test in scanTests)
            {
                string[]? args = test.Value.Args?.Split(' ');
                DrawTestHeader(c, test.Key);
                DrawUsedArgs(args);

                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    WorkingDirectory = @"..\..\..\..\Launcher",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };
                psi.ArgumentList.Add("run");
                psi.ArgumentList.Add("--no-build");
                psi.ArgumentList.Add("--");
                psi.ArgumentList.Add("scan");
                psi.ArgumentList.Add(targetDir);
                psi.ArgumentList.Add("--out-dir");
                psi.ArgumentList.Add(outDir);
                if (!args.Contains("--type"))
                {
                    psi.ArgumentList.Add("-t");
                    psi.ArgumentList.Add("text");
                }
                if (args != null)
                {
                    foreach (var arg in args)
                        psi.ArgumentList.Add(arg);
                }

                using var process = Process.Start(psi)!;

                string stdout = await process.StandardOutput.ReadToEndAsync();
                string stderr = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                int exitCode = process.ExitCode;
                if (exitCode == test.Value.ExpectedCode)
                    WriteColor($"Exit code: {exitCode}", ConsoleColor.Green);
                else
                {
                    WriteColor($"Exit code: {exitCode}", ConsoleColor.Red);
                    success = false;
                }
                if (!string.IsNullOrWhiteSpace(stdout))
                {
                    Console.WriteLine("--- stdout ---");
                    Console.WriteLine(stdout);
                }

                if (!string.IsNullOrWhiteSpace(stderr))
                {
                    Console.WriteLine("--- stderr ---");
                    Console.WriteLine(stderr);
                }
                c++;
            }

            // finish
            sw.Stop();
            DrawLine('=', 20);
            WriteMsg("Cleaning up ...", MsgType.Info);
            if (Directory.Exists(outDir))
                Directory.Delete(outDir, true);
            WriteMsg("Done!", MsgType.Success);
            DrawLine('=', 20);
            WriteMsg("TESTS FINISHED", MsgType.Info);
            WriteColor(
                "Overall Result: " + (success ? "OK" : "FAIL"),
                success ? ConsoleColor.Green : ConsoleColor.Red
            );
            Console.WriteLine(
                "Elapsed Seconds: " + (sw.ElapsedMilliseconds / 1000).ToString("#0.00")
            );
            Console.WriteLine("Tests Performed: " + (c - 1));
            DrawLine('=', 20);

            // helpers
            void DrawDefaults()
            {
                DrawLine('=', 20);
                WriteColor("Default Arguments Used:", ConsoleColor.Green);
                WriteColor("Target: ", ConsoleColor.Cyan, newLine: false);
                Console.WriteLine(targetDir);
                WriteColor("Output Dir: ", ConsoleColor.Cyan, newLine: false);
                Console.WriteLine(outDir);
                DrawLine('=', 20);
            }
            void DrawTestHeader(int id, string title)
            {
                WriteColor($"[{id}]", ConsoleColor.Magenta, newLine: false);
                WriteColor($" {title} ", ConsoleColor.Yellow, newLine: false);
            }
            void DrawUsedArgs(string[]? args)
            {
                WriteColor("Args:", ConsoleColor.Cyan, newLine: false);
                if (args != null)
                    foreach (string arg in args)
                        Console.Write(' ' + arg);
                Console.WriteLine();
            }
        }
    }
}
