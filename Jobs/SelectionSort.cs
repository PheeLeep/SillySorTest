namespace SillySorTest.Jobs
{
    public class SelectionSort : SortJobAbstract
    {

        public override string Name => "Selection Sort";

        public override string CmdName => "selection";

        public override bool IsJokeType => false;

        public override void Run(List<int> item)
        {
            for (int i = 0; i < item.Count; i++)
            {
                int min = i;
                for (int j = i + 1; j < item.Count; j++)
                {
                    AccessCount += 2; // accessing item[j] and item[min]
                    ComparisonCount++;

                    if (item[j] < item[min])
                    {
                        min = j;
                    }
                }

                if (min != i)
                {
                    int temp = item[i];
                    item[i] = item[min];
                    item[min] = temp;

                    ChangeCount += 3;
                }
            }
        }
    }
}