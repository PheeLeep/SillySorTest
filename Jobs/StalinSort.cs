namespace SillySorTest.Jobs;

public class StalinSort : SortJobAbstract
{
    public override bool IsJokeType => true;

    public override string Name => "Stalin Sort";

    public override string CmdName => "stalin";

    public override void Run(List<int> item)
    {

        List<int> result = new List<int>();
        int last = item[0];
        AccessCount++;

        result.Add(last);

        for (int i = 1; i < item.Count; i++)
        {
            int current = item[i];
            AccessCount++;

            if (current >= last)
            {
                ChangeCount++;
                result.Add(current);
                last = current;
            }
            ComparisonCount++;
        }

    }
}
