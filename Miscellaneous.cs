using System;
using Spectre.Console;

namespace SillySorTest;

public static class Miscellaneous
{
    public static void PrintErrorTable(List<SortResultClass> errors)
    {
        if (errors.Count > 0)
        {
            Console.WriteLine("[X]: There is an error occurred during the test.");
            var errTable = new Table();
            errTable.AddColumn("Test");
            errTable.AddColumn("Message");

            foreach (var a in errors)
            {
                errTable.AddRow(a.TestName, a.Message!);
            }

            AnsiConsole.Write(errTable);
        }
        else
        {
            Console.WriteLine("");
        }
    }

    public static void PrintTable(List<SortResultClass> results)
    {
        var table = new Table();
        table.AddColumns(["Test", "Mem. Consumption", "Time Elapsed", "Access", "Changes", "Compare"]);
        foreach (var a in results)
        {
            table.AddRow(a.TestName,
                         ToHumanReadableSize(a.PostTestMemConsumption - a.PreTestMemConsumption),
                          a.Time,
                          a.AccessCount.ToString(),
                          a.ChangeCount.ToString(),
                          a.ComparisonCount.ToString());
        }

        AnsiConsole.Write(table);
        Console.WriteLine();
    }

    public static string ToHumanReadableSize(decimal bytes)
    {
        string[] suffixes = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];

        if (bytes == 0)
            return "0 B";

        bool isNegative = bytes < 0;
        decimal value = Math.Abs(bytes);

        int i = 0;

        while (value >= 1024 && i < suffixes.Length - 1)
        {
            value /= 1024;
            i++;
        }

        string result = $"{value:0.##} {suffixes[i]}";
        return isNegative ? "-" + result : result;
    }
}
