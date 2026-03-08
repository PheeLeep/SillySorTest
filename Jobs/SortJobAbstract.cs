namespace SillySorTest.Jobs
{
    public abstract class SortJobAbstract
    {
        public abstract string Name { get; } 
        public abstract string CmdName { get; }
        public abstract bool IsJokeType { get; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public long PreTestMemConsumption { get; set; }
        public long PostTestMemConsumption { get; set; }

        public decimal AccessCount { get; set; }
        public decimal ComparisonCount { get; set; }
        public decimal ChangeCount { get; set; }

        public abstract void Run(List<int> item);
    }
}