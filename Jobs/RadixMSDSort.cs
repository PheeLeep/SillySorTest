using System;

namespace SillySorTest.Jobs;

public class RadixMSDSort : SortJobAbstract
{
    public override string Name => "Radix MSD Sort";

    public override string CmdName => "radixmsd";

    public override bool IsJokeType => false;

    public override void Run(List<int> item)
    {
        int max = item.Max();
        AccessCount++;

        for (int exp = 1; max / exp > 0; exp *= 10)
        {
            CountSort(item, exp);
        }
    }

    private void CountSort(List<int> item, int exp)
    {
        int n = item.Count;
        int[] output = new int[n];
        int[] count = new int[10];

        for (int i = 0; i < n; i++)
        {
            count[item[i] / exp % 10]++;
            AccessCount++;
        }

        for (int i = 1; i < 10; i++)
        {
            count[i] += count[i - 1];
        }

        for (int i = n - 1; i >= 0; i--)
        {
            output[count[item[i] / exp % 10] - 1] = item[i];
            AccessCount++;
            count[item[i] / exp % 10]--;
        }

        for (int i = 0; i < n; i++)
        {
            item[i] = output[i];
            AccessCount++;
            ChangeCount++;
        }
    }
}
