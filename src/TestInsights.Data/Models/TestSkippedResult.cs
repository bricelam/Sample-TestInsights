namespace TestInsights.Models
{
    public class TestSkippedResult : TestResult
    {
        private string _reason;

        public string Reason
        {
            get { return _reason; }
            set { Set(nameof(Reason), ref _reason, value); }
        }
    }
}
