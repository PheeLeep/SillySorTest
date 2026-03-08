using System;
using System.Diagnostics;
using System.Text.Json;
using ArgSharp;

namespace SillySorTest;

public class JobHandler
{
    public async Task<SortResultClass> Run(string name)
    {
        var useJoke = ArgSharpClass.GetValue<bool>("--joke");
        var sortSize = ArgSharpClass.GetValue<int>("-s");
        
        var procInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"SillySorTest.dll -s {sortSize}{(useJoke ? " --joke" : "")} -j {name}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var proc = Process.Start(procInfo))
        {
            if (proc is null)
            {
                return new SortResultClass
                {
                    TestName = name,
                    Success = false,
                    Message = "Process is null"
                };
            }
            string json = await proc.StandardOutput.ReadToEndAsync();
            await proc.WaitForExitAsync();

            try
            {
                var res = JsonSerializer.Deserialize<SortResultClass>(json);
                if (res is null)
                {
                    return new SortResultClass
                    {
                        TestName = name,
                        Success = false,
                        Message = "Result is empty."
                    };
                }
                res.TestName = name;
                return res;
            }
            catch (Exception ex)
            {
                return new SortResultClass
                {
                    TestName = name,
                    Success = false,
                    Message = ex.Message
                };
            }
        }

    }
}
