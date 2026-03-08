using System;
using System.Collections.Generic;

namespace SillySorTest.Jobs;

public class MergeSort : SortJobAbstract
{

    public override string Name => "Merge Sort";

    public override string CmdName => "merge";

    public override bool IsJokeType => false;

    public override void Run(List<int> item)
    {
        var sorted = MergeSortInternal(item);
        item.Clear();
        item.AddRange(sorted);
    }

    private List<int> MergeSortInternal(List<int> arr)
    {
        if (arr.Count < 2) return new List<int>(arr);

        int mid = arr.Count / 2;
        List<int> left = arr.GetRange(0, mid);
        List<int> right = arr.GetRange(mid, arr.Count - mid);

        // Count accesses for creating sublists
        AccessCount += arr.Count; // rough estimate for copying

        List<int> sortedLeft = MergeSortInternal(left);
        List<int> sortedRight = MergeSortInternal(right);

        return Merge(sortedLeft, sortedRight);
    }

    private List<int> Merge(List<int> left, List<int> right)
    {
        List<int> merged = new List<int>();
        int li = 0, ri = 0;

        while (li < left.Count && ri < right.Count)
        {
            AccessCount += 2;       // accessing left[li] and right[ri]
            ComparisonCount++;      // comparing elements
            if (left[li] < right[ri])
            {
                merged.Add(left[li]);
                ChangeCount++;      // adding element
                li++;
            }
            else
            {
                merged.Add(right[ri]);
                ChangeCount++;      // adding element
                ri++;
            }
        }

        // Add remaining elements
        while (li < left.Count)
        {
            merged.Add(left[li]);
            AccessCount++;
            ChangeCount++;
            li++;
        }

        while (ri < right.Count)
        {
            merged.Add(right[ri]);
            AccessCount++;
            ChangeCount++;
            ri++;
        }

        return merged;
    }
}