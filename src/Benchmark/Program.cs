using System.Diagnostics;
using Core;
using Core.Utils;
using Core.Writers;
using static CLI.Display;
using static Core.Settings;

namespace Benchmark
{
    internal class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CoreException _ex = new(ExitCode.Success, ExitMessages.Get(ExitCode.Success));
            GenFlags flags = new GenFlags();
            string tempDir = Path.GetTempPath(),
                tempPath = Path.Combine(tempDir, "filetreegen_benchmark"),
                benchmarksDir = "../../../../../benchmarks",
                csvPath = Path.Combine(
                    benchmarksDir,
                    $"benchmark_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv"
                );
            if (!Directory.Exists(benchmarksDir))
                Directory.CreateDirectory(benchmarksDir);
            TestData[] dataset =
            {
                new("Tiny", 100, 100, 3),
                new("Small", 1_000, 500, 4),
                new("Medium", 10_000, 1_000, 6),
                new("Large", 100_000, 10_000, 10),
            };
            try
            {
                await PerformTests(dataset, tempPath);
            }
            catch (CoreException ex)
            {
                _ex = ex;
            }
            catch (Exception newEx)
            {
                _ex = ExitMessages.TranslateOSException(newEx);
            }
            finally
            {
                if (_ex.Code != ExitCode.Success)
                {
                    WriteMsg("[CODE]: " + (int)_ex.Code + $" ({_ex.Code})", MsgType.Error);
                    WriteMsg("[DESCRIPTION]: " + _ex.Message, MsgType.Error);
                    if (_ex.InnerException != null)
                    {
                        WriteMsg("[INNER MESSAGE]:\n" + _ex.InnerException.Message, MsgType.Error);
                        WriteMsg(
                            "[INNER STACK TRACE]:\n" + _ex.InnerException.StackTrace,
                            MsgType.Error
                        );
                    }
                }
                Environment.Exit((int)_ex.Code);
            }

            async Task PerformTests(TestData[] data, string rootPath)
            {
                Stopwatch sw = new();
                using StreamWriter csvWriter = new(csvPath);

                csvWriter.WriteLine(
                    "Data Set;Files;Folders;Depth;Report Type;Time (Ms);Peak Ram (MB);Report Size (KB);Files/Second"
                );
                flags.outDir = rootPath;
                for (int i = 0; i < data.Length; i++)
                {
                    // STRUCTURE GENERATION
                    TestData set = data[i];
                    WriteColor($"[ TEST ] ", ConsoleColor.Magenta, newLine: false);
                    Console.Write($"[{i + 1}/{data.Length}] ");
                    WriteColor($"[{set.Label}] ", ConsoleColor.Yellow, newLine: false);
                    Console.WriteLine(
                        $"FILES: {set.Files} | DIRS: {set.Dirs} | DEPTH: {set.MaxDepth}"
                    );
                    WriteMsg($"Generating structure for test: " + (i + 1), MsgType.Info);
                    string testPath = StructureBuilder.Build(
                        rootPath,
                        set.Files,
                        set.Dirs,
                        set.MaxDepth
                    );
                    WriteMsg(
                        $"Structure generated '{Path.GetFileName(testPath)}'",
                        MsgType.Success
                    );

                    // REPORT GENERATIONS
                    WriteMsg($"Performing test: " + (i + 1), MsgType.Warning);
                    foreach (ReportType type in Enum.GetValues<ReportType>())
                    {
                        flags.reportType = type;
                        flags.targetDir = testPath;
                        // MEASUREMENTS
                        Process process = Process.GetCurrentProcess();
                        long peakRam = 0;
                        CancellationTokenSource cts = new();
                        Thread monitor = new(() =>
                        {
                            while (!cts.Token.IsCancellationRequested)
                            {
                                process.Refresh();
                                peakRam = Math.Max(peakRam, process.WorkingSet64);
                                Thread.Sleep(100);
                            }
                        });
                        monitor.Start();

                        // benchmark
                        sw.Start();
                        await GenerateReport(flags);
                        // collect data
                        sw.Stop();
                        cts.Cancel();
                        monitor.Join();

                        string outPath = ReportInfo.GeneratePath(
                            flags.outDir,
                            flags.targetDir,
                            flags.reportType,
                            flags.reportNameScheme
                        );
                        FileInfo outFile = new FileInfo(outPath);
                        long size = outFile.Length / 1024,
                            ram = peakRam / 1024 / 1024;
                        // Data Set | Files | Folders | Depth | Report Type | Time (Ms) | Peak Ram (MB) | Report Size (KB) | Files/Second
                        csvWriter.WriteLine(
                            set.Label
                                + ';'
                                + set.Files
                                + ';'
                                + set.Dirs
                                + ';'
                                + set.MaxDepth
                                + ';'
                                + type
                                + ';'
                                + sw.ElapsedMilliseconds
                                + ';'
                                + ram
                                + ';'
                                + size
                                + ';'
                                + ((double)set.Files / sw.ElapsedMilliseconds * 1000).ToString(
                                    "#0.00"
                                )
                        );
                        WriteMsg($"{type} - {sw.ElapsedMilliseconds} ms", MsgType.Success);
                        sw.Reset();
                    }
                }
                WriteMsg("Cleaning up...", MsgType.Info);
                Directory.Delete(rootPath, true);
                FileSystem.OpenPath(csvPath);
                WriteMsg("BENCHMARK FINISHED", MsgType.Success);
            }
        }

        record struct TestData(string Label, int Files, int Dirs, int MaxDepth);

        static async Task GenerateReport(GenFlags flags)
        {
            switch (flags.reportType)
            {
                case ReportType.HTML:
                    HtmlWriter hmtlWriter = new HtmlWriter(flags);
                    await hmtlWriter.WriteAsync();
                    break;

                case ReportType.Markdown:
                    MarkdownWriter mdWriter = new MarkdownWriter(flags);
                    await mdWriter.WriteAsync();
                    break;

                case ReportType.Text:
                    Core.Writers.TextWriter txtWriter = new Core.Writers.TextWriter(flags);
                    await txtWriter.WriteAsync();
                    break;

                default:
                    break;
            }
        }
    }
}
