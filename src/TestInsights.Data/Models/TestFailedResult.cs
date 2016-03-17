namespace TestInsights.Models
{
    public class TestFailedResult : TestResult
    {
        public string ExceptionType { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
