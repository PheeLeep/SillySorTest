using System;

namespace SillySorTest.Jobs;

public class QuickSort : SortJobAbstract
{
    public override string Name => "Quick Sort";

    public override string CmdName => "quicksort";

    public override bool IsJokeType => false;

    public override void Run(List<int> item)
    {
        QuickSortRecursive(item, 0, item.Count - 1);
    }

    private void QuickSortRecursive(List<int> item, int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(item, low, high);

            QuickSortRecursive(item, low, pi - 1);
            QuickSortRecursive(item, pi + 1, high);
        }
    }

    private int Partition(List<int> item, int low, int high)
    {
        int pivot = item[high];
        AccessCount++;

        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            AccessCount++;
            ComparisonCount++;

            if (item[j] < pivot)
            {
                i++;
                Swap(item, i, j);
            }
        }

        Swap(item, i + 1, high);
        return i + 1;
    }

    private void Swap(List<int> item, int i, int j)
    {
        int temp = item[i];
        AccessCount++;

        item[i] = item[j];
        AccessCount++;
        ChangeCount++;

        item[j] = temp;
        ChangeCount++;
    }
}
