using System;

namespace SillySorTest.Jobs;

public class HeapSort : SortJobAbstract
{
    public override string Name => "Heap Sort";

    public override string CmdName => "heap";

    public override bool IsJokeType => false;

    public override void Run(List<int> item)
    {
        int n = item.Count;

        for (int i = n / 2 - 1; i >= 0; i--)
            Heapify(item, n, i);

        for (int i = n - 1; i > 0; i--)
        {
            int temp = item[0];
            AccessCount++;

            item[0] = item[i];
            AccessCount++;
            ChangeCount++;

            item[i] = temp;
            ChangeCount++;

            Heapify(item, i, 0);
        }
    }

    private void Heapify(List<int> item, int n, int i)
    {
        int largest = i;
        int left = 2 * i + 1;
        int right = 2 * i + 2;

        if (left < n && item[left] > item[largest])
        {
            ComparisonCount++;
            largest = left;
        }

        if (right < n && item[right] > item[largest])
        {
            ComparisonCount++;
            largest = right;
        }

        if (largest != i)
        {
            int swap = item[i];
            AccessCount++;

            item[i] = item[largest];
            AccessCount++;
            ChangeCount++;

            item[largest] = swap;
            ChangeCount++;

            Heapify(item, n, largest);
        }
    }

}
