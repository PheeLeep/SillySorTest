using System;

namespace SillySorTest.Jobs;

public class BogoSort : SortJobAbstract
{

    public override string Name => "Bogo Sort";

    public override string CmdName => "bogo";

    public override bool IsJokeType => true;

    public override void Run(List<int> item)
    {
        Random rand = new Random();

        while (!IsSorted(item))
        {
            for (int i = 0; i < item.Count; i++)
            {
                int j = rand.Next(0, item.Count);
                int temp = item[i];
                AccessCount++;

                item[i] = item[j];
                AccessCount++;
                ChangeCount++;

                item[j] = temp;
                ChangeCount++;
            }
        }
    }

    private bool IsSorted(List<int> item)
    {
        for (int i = 1; i < item.Count; i++)
        {
            if (item[i - 1] > item[i])
            {
                ComparisonCount++;
                return false;
            }
        }

        return true;
    }


}
