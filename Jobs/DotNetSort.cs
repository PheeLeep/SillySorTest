using System;

namespace SillySorTest.Jobs;

public class DotNetSort : SortJobAbstract
{

    public override string Name => ".NET Sort";

    public override string CmdName => "dotnetsort";
    public override bool IsJokeType => false;


    public override void Run(List<int> item)
    {
        AccessCount = -1;
        ChangeCount = -1;

        item.Sort(new CountingComparer(this));
    }

    private class CountingComparer : IComparer<int>
    {
        private readonly DotNetSort _job;

        public CountingComparer(DotNetSort job)
        {
            _job = job;
        }

        public int Compare(int x, int y)
        {
            _job.ComparisonCount++;
            return x.CompareTo(y);
        }
    }
}
