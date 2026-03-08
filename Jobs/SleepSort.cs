using System;

namespace SillySorTest.Jobs;

public class SleepSort : SortJobAbstract
{

    public override string Name => "Sleep Sort";

    public override string CmdName => "sleep";

    public override bool IsJokeType => true;

    public override void Run(List<int> item)
    {
        List<int> sorted = new List<int>();
        List<Task> tasks = new List<Task>();

        foreach (var num in item)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(num * 10); // Sleep for num * 10 milliseconds
                lock (sorted)
                {
                    sorted.Add(num);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Copy sorted back to original list
        for (int i = 0; i < item.Count; i++)
        {
            item[i] = sorted[i];
        }
    }

}
