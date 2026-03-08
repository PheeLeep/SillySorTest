using System;

namespace SillySorTest;

public class SortResultClass
{
    public string TestName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Message { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public long PreTestMemConsumption { get; set; }
    public long PostTestMemConsumption { get; set; }
    public decimal AccessCount { get; set; }
    public decimal ComparisonCount { get; set; }
    public decimal ChangeCount { get; set; }
    public string Time { get; set; } = string.Empty;
}
