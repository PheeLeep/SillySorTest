using System;

namespace SillySorTest.Jobs;

public class BubbleSort : SortJobAbstract
{

    public override string Name => "Bubble Sort";

    public override string CmdName => "bubble";

    public override bool IsJokeType => false;

    public override void Run(List<int> item)
    {
        for (int i = 0; i < item.Count; i++)
        {
            for (int j = 0; j < item.Count - i - 1; j++)
            {
                if (item[j] > item[j + 1])
                {
                    ComparisonCount++;

                    int temp = item[j];
                    AccessCount++;

                    item[j] = item[j + 1];
                    AccessCount++;
                    ChangeCount++;

                    item[j + 1] = temp;
                    ChangeCount++;
                }
            }
        }
    }
}
