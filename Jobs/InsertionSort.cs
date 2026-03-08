using System;

namespace SillySorTest.Jobs;

public class InsertionSort : SortJobAbstract
{

    public override string Name => "Insertion Sort";

    public override string CmdName => "insertion";

    public override bool IsJokeType => false;

    public override void Run(List<int> item)
    {
        for (int i = 0; i < item.Count; i++)
        {
            int current = item[i];
            AccessCount++;

            int j = i - 1;

            while (j > -1 && current < item[j])
            {
                AccessCount++;
                ComparisonCount++;
                item[j + 1] = item[j];
                AccessCount++;
                ChangeCount++;
                j--;
            }

            item[j + 1] = current;
            ChangeCount++;
        }
    }
}
