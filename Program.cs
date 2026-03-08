using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using ArgSharp;
using SillySorTest.Jobs;
using Spectre.Console;

namespace SillySorTest;

class Program
{

    static bool useJoke = false;
    static int sortSize = 1024;
    static bool parallelMode = false;
    static bool usePreciseRNG = false;



    enum JobStatus { Waiting, Running, Done, Failed }

    record JobProgress(string Name)
    {
        public JobStatus Status { get; set; } = JobStatus.Waiting;
        public string Elapsed { get; set; } = "-";
        public string Time { get; set; } = "-";
        public Stopwatch Stopwatch { get; } = new();
    }
    static readonly SortJobAbstract[] jobs =
    [
        new StalinSort(),
        new DotNetSort(),
        new BubbleSort(),
        new InsertionSort(),
        new MergeSort(),
        new SelectionSort(),
        new QuickSort(),
        new RadixLSDSort(),
        new RadixMSDSort(),
        new BogoSort(),
        new SleepSort(),
        new HeapSort()
    ];

    static async Task Main(string[] args)
    {
        Init();
        if (!ArgSharpClass.Parse(args)) return;

        List<string> tests = new List<string>();

        parallelMode = ArgSharpClass.GetValue<bool>("-p");
        usePreciseRNG = ArgSharpClass.GetValue<bool>("--precise");
        useJoke = ArgSharpClass.GetValue<bool>("--joke");
        sortSize = ArgSharpClass.GetValue<int>("-s");

        if (sortSize < 1)
        {
            AnsiConsole.MarkupLine("[red][[X]][/]: Sort size must be at least 1.");
            Environment.Exit(1);
            return;
        }

        string jobName = ArgSharpClass.GetValue<string>("-j");
        if (!string.IsNullOrEmpty(jobName))
        {
            PerformTest(jobName);
            return;
        }
        string excludeParam = ArgSharpClass.GetValue<string>("--exclude");

        // use comma to separate multiple job names, or just space (handled in loop)
        string[] excludeJobs = excludeParam.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries);

        foreach (var kvp in jobs)
        {
            var name = kvp.CmdName;

            if (!useJoke && kvp.IsJokeType)
                continue;

            tests.Add(name);
        }

        if (excludeJobs.Length > 0)
        {
            for (int i = 0; i < excludeJobs.Length; i++)
            {
                string name = excludeJobs[i];
                tests.Remove(name);
            }

        }

        if (tests.Count == 0)
        {
            Console.WriteLine("[X]: Job tests not found.");
            Environment.Exit(1);
            return;
        }

        if (tests.Contains("bogo") && useJoke)
        {
            // Make it a prompt to begin
            var proceed = AnsiConsole.Prompt(
                new ConfirmationPrompt("[red][[WTF?!!]][/] Are you about to run BogoSort frfr? 😭😭💀🥀")
                    .ShowDefaultValue()
            );
            if (!proceed)
            {
                Console.WriteLine("[i]: Test cancelled.");
                Environment.Exit(0);
                return;
            }
        }

        Console.WriteLine($"[i]: Test started ({tests.Count} test/s [{(parallelMode ? "Parallel Mode" : "Serial Mode")}])");
        Console.WriteLine($"[i]: uses {(usePreciseRNG ? "Secure RNG (increases memory usage)" : "Pseudo RNG")}");

        List<SortResultClass> sortResults = new List<SortResultClass>();


        var liveTable = new Table()
            .AddColumn("Job")
            .AddColumn("Status");

        var progressMap = tests.ToDictionary(
                            name => name,
                            name => new JobProgress(name)
                            );

        await AnsiConsole.Live(liveTable)
            .StartAsync(async ctx =>
            {
                // Rebuild table rows on each tick
                void Refresh()
                {
                    liveTable.Rows.Clear();
                    foreach (var p in progressMap.Values)
                    {
                        var (statusColor, statusText) = p.Status switch
                        {
                            JobStatus.Running => ("yellow", "⏳ Running"),
                            JobStatus.Done => ("green", "✅ Done"),
                            JobStatus.Failed => ("red", "❌ Failed"),
                            _ => ("grey", "⬜ Waiting"),
                        };
                        liveTable.AddRow(
                            p.Name,
                            $"[{statusColor}]{statusText}[/] {(p.Status == JobStatus.Running ? p.Stopwatch.Elapsed.ToString(@"mm\:ss\.fff") : "")}"
                        );
                    }
                    ctx.Refresh();
                }

                // Launch jobs
                IEnumerable<Task<SortResultClass>> tasks;
                if (parallelMode)
                {
                    tasks = progressMap.Keys.Select(name =>
                    {
                        var p = progressMap[name];
                        p.Status = JobStatus.Running;
                        p.Stopwatch.Start();
                        return new JobHandler().Run(name).ContinueWith(t =>
                        {
                            p.Stopwatch.Stop();
                            p.Status = t.Result.Success ? JobStatus.Done : JobStatus.Failed;
                            p.Time = t.Result.Time;
                            return t.Result;
                        });
                    }).ToList();
                }
                else
                {
                    // Serial: kick off one at a time
                    var serialResults = new List<Task<SortResultClass>>();
                    var runSerial = Task.Run(async () =>
                    {
                        foreach (var name in progressMap.Keys)
                        {
                            var p = progressMap[name];
                            p.Status = JobStatus.Running;
                            p.Stopwatch.Start();
                            var result = await new JobHandler().Run(name);
                            p.Stopwatch.Stop();
                            p.Status = result.Success ? JobStatus.Done : JobStatus.Failed;
                            p.Time = result.Time;
                            sortResults.Add(result);
                        }
                    });
                    tasks = serialResults; // not used for serial, handled above

                    // Refresh loop while serial runs
                    while (!runSerial.IsCompleted)
                    {
                        Refresh();
                        await Task.Delay(100);
                    }
                    await runSerial;
                    Refresh();
                    return;
                }

                // Parallel refresh loop
                while (tasks.Any(t => !t.IsCompleted))
                {
                    Refresh();
                    await Task.Delay(100);
                }
                sortResults = (await Task.WhenAll(tasks)).ToList();
                Refresh(); // final update
            });

        Console.WriteLine("[i]: Test Completed.");

        Miscellaneous.PrintErrorTable(sortResults.Where(a => !a.Success).ToList());
        Console.WriteLine("Results");
        Miscellaneous.PrintTable(sortResults.Where(s => s.Success).OrderBy(a => a.TestName).ToList());

        Console.WriteLine("Memory Consumption:");
        var memComparison = sortResults.Where(r => r.Success).Select(r => new
        {
            r.TestName,
            MemDiff = r.PostTestMemConsumption - r.PreTestMemConsumption
        })
            .OrderBy(r => r.MemDiff)
            .ToList();
        var bestMem = memComparison.First();
        var worstMem = memComparison.Last();
        Console.WriteLine($"  - Best: {bestMem.TestName} ({Miscellaneous.ToHumanReadableSize(bestMem.MemDiff)} or {bestMem.MemDiff} bytes)");
        Console.WriteLine($"  - Worst: {worstMem.TestName} ({Miscellaneous.ToHumanReadableSize(worstMem.MemDiff)} or {worstMem.MemDiff} bytes)");

        Console.WriteLine("Time:");
        var timeComparison = sortResults.Where(r => r.Success).Select(r => new
        {
            r.TestName,
            TimeSpan = TimeSpan.Parse(r.Time)
        })
            .OrderBy(r => r.TimeSpan)
            .ToList();
        var bestTime = timeComparison.First();
        var worstTime = timeComparison.Last();
        Console.WriteLine($"  - Best: {bestTime.TestName} ({bestTime.TimeSpan})");
        Console.WriteLine($"  - Worst: {worstTime.TestName} ({worstTime.TimeSpan})");

        Console.WriteLine("Least Access Count:");
        var accessComparison = sortResults.Where(r => r.Success && r.AccessCount > 0).Select(r => new
        {
            r.TestName,
            r.AccessCount
        })
            .OrderBy(r => r.AccessCount)
            .ToList();
        var bestAccess = accessComparison.First();
        var worstAccess = accessComparison.Last();
        Console.WriteLine($"  - Best: {bestAccess.TestName} ({bestAccess.AccessCount} accesses)");
        Console.WriteLine($"  - Worst: {worstAccess.TestName} ({worstAccess.AccessCount} accesses)");

        Console.WriteLine("Least Change Count:");
        var changeComparison = sortResults.Where(r => r.Success && r.ChangeCount > 0).Select(r => new
        {
            r.TestName,
            r.ChangeCount
        })
            .OrderBy(r => r.ChangeCount)
            .ToList();
        var bestChange = changeComparison.First();
        var worstChange = changeComparison.Last();
        Console.WriteLine($"  - Best: {bestChange.TestName} ({bestChange.ChangeCount} changes)");
        Console.WriteLine($"  - Worst: {worstChange.TestName} ({worstChange.ChangeCount} changes)");

        Console.WriteLine("Least Comparison Count:");
        var comparisonComparison = sortResults.Where(r => r.Success && r.ComparisonCount > 0).Select(r => new
        {
            r.TestName,
            r.ComparisonCount
        })
            .OrderBy(r => r.ComparisonCount)
            .ToList();
        var bestComparison = comparisonComparison.First();
        var worstComparison = comparisonComparison.Last();
        Console.WriteLine($"  - Best: {bestComparison.TestName} ({bestComparison.ComparisonCount} comparisons)");
        Console.WriteLine($"  - Worst: {worstComparison.TestName} ({worstComparison.ComparisonCount} comparisons)");

        var gcInfo = GC.GetGCMemoryInfo();

        long totalRam = gcInfo.TotalAvailableMemoryBytes;

        Console.WriteLine("\nSystem Specification:");
        Console.WriteLine($"  - CPU: {GetProcessorName()} ({Environment.ProcessorCount} logical cores, {(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")})");
        Console.WriteLine($"  - OS: {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}");
        Console.WriteLine($"  - .NET Version: {Environment.Version}");
        Console.WriteLine($"  - Total RAM: {Miscellaneous.ToHumanReadableSize(totalRam)} ({totalRam} bytes)");
    }
    static void Init()
    {

        ArgSharpClass.ArgumentZeroAction = ArgSharpClass.ArgZeroAction.ShowHelp;
        ArgSharpClass.Init(AppDomain.CurrentDomain.FriendlyName);
        ArgSharpClass.AddArgument<bool>(["-p"], helpMsg: "Run test as parallel.");
        ArgSharpClass.AddArgument<bool>(["--precise"], helpMsg: "Use Secure RNG instead of pseudo RNG.\n(true random but can increase the memory usage, depending on the array size set)", defaultValue: false);
        ArgSharpClass.AddArgument<int>(["-s"], helpMsg: "Setting the size of array to use. (Default: 1024)", defaultValue: 1024);
        ArgSharpClass.AddArgument<bool>(["--joke"], helpMsg: "Allow also to use joke type sort test jobs.", defaultValue: false);
        ArgSharpClass.AddArgument<string>(["-j"], "job name", helpMsg: "Performs only one sort job, the program will output as JSON (Should be use on last parameter)\n" +
                                              "To run joke-type, must include --joke before this parameter.");
        ArgSharpClass.AddArgument<string>(["--exclude"], "", "Excludes one or more sort test jobs.\n" +
                                                    "To specify multiple jobs, separate them with comma. (e.g. --exclude merge,sort)",
                                                    "");


        ArgSharpClass.OnHelpInvoked = new Action(() =>
        {
            PrintHelp();
            Environment.Exit(0);
        });
    }
    static void PerformTest(string name)
    {
        var job = jobs.SingleOrDefault(a => a.CmdName == name);
        if (job is null)
        {
            Console.WriteLine(JsonSerializer.Serialize(new SortResultClass
            {
                Message = $"No job found.",
                Success = false
            }));
            return;
        }

        if (!useJoke && job.IsJokeType)
        {
            Console.WriteLine(JsonSerializer.Serialize(new SortResultClass
            {
                Message = $"Use --joke to run '{name}'.",
                Success = false
            }));
            return;
        }

        long peakMemory = 0;
        bool sortingDone = false;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        job.PreTestMemConsumption = GC.GetTotalMemory(true);
        var memoryTracker = Task.Run(() =>
        {
            while (!sortingDone)
            {
                long current = GC.GetTotalMemory(false);
                if (current > peakMemory) peakMemory = current;
                Thread.Sleep(1); // check every millisecond
            }
        });

        Random rngesus = new Random();

        List<int> item = new List<int>();
        for (int i = 0; i < sortSize; i++)
        {
            // Generate a non-negative int in [0, int.MaxValue)
            int number = usePreciseRNG ? RandomNumberGenerator.GetInt32(int.MaxValue) : rngesus.Next(0, int.MaxValue);
            item.Add(number);
        }

        job.StartTime = DateTime.UtcNow;
        job.Run(item);

        job.EndTime = DateTime.UtcNow;
        sortingDone = true;
        memoryTracker.Wait();
        job.PostTestMemConsumption = peakMemory;
        var time = job.EndTime - job.StartTime;


        var result = new SortResultClass
        {
            Message = "Success",
            Success = true,
            StartTime = job.StartTime,
            EndTime = job.EndTime,
            AccessCount = job.AccessCount,
            ChangeCount = job.ChangeCount,
            ComparisonCount = job.ComparisonCount,
            PreTestMemConsumption = job.PreTestMemConsumption,
            PostTestMemConsumption = job.PostTestMemConsumption,
            Time = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}"
        };

        string json = JsonSerializer.Serialize(result);
        Console.WriteLine(json);
    }



    static void PrintHelp()
    {
        Console.WriteLine("Sort Test Jobs:\n");

        // Separate normal and joke jobs
        var normalJobs = new List<string>();
        var jokeJobs = new List<string>();

        foreach (var kvp in jobs)
        {
            var name = kvp.CmdName;

            if (kvp.IsJokeType)
                jokeJobs.Add(name);
            else
                normalJobs.Add(name);
        }

        // Print normal jobs
        Console.WriteLine("Available Jobs:");
        foreach (var name in normalJobs)
        {
            Console.WriteLine($"  - {name}");
        }

        // Print joke jobs in a separate section
        if (jokeJobs.Count > 0)
        {
            Console.WriteLine("\nJoke Jobs:");
            foreach (var name in jokeJobs)
            {
                Console.WriteLine($"  - {name}");
            }
        }
    }


    static string GetProcessorName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "Unknown";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var lines = File.ReadAllLines("/proc/cpuinfo");
            return lines.FirstOrDefault(l => l.StartsWith("model name"))?.Split(':')[1].Trim() ?? "Unknown";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var info = new ProcessStartInfo("sysctl", "-n machdep.cpu.brand_string")
            { RedirectStandardOutput = true };
            return Process.Start(info)?.StandardOutput.ReadToEnd().Trim() ?? "Unknown";
        }
        return "Unknown";
    }
}
