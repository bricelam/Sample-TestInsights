namespace TestInsights.Data
{
    public class TestFailed : TestResult
    {
        public string ExceptionType { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
