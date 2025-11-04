namespace TestManiac.Core;

/// <summary>
/// Result of a test interaction
/// </summary>
public class TestResult
{
    public string Url { get; set; } = string.Empty;
    public string ElementDescription { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorType { get; set; }
    public DateTime Timestamp { get; set; }
    public string? ScreenshotPath { get; set; }
}

/// <summary>
/// Summary of the entire test run
/// </summary>
public class TestSummary
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int TotalPagesVisited { get; set; }
    public int TotalInteractions { get; set; }
    public int SuccessfulInteractions { get; set; }
    public int FailedInteractions { get; set; }
    public List<TestResult> Results { get; set; } = new();
    public List<string> VisitedUrls { get; set; } = new();

    public TimeSpan Duration => EndTime - StartTime;

    public double SuccessRate => TotalInteractions > 0
        ? (double)SuccessfulInteractions / TotalInteractions * 100
        : 0;
}
