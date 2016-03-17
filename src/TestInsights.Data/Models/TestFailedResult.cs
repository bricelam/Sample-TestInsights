namespace TestInsights.Models
{
    public class TestFailedResult : TestResult
    {
        private string _exceptionType;
        private string _message;
        private string _stackTrace;

        public string ExceptionType
        {
            get { return _exceptionType; }
            set { Set(nameof(ExceptionType), ref _exceptionType, value); }
        }

        public string Message
        {
            get { return _message; }
            set { Set(nameof(Message), ref _message, value); }
        }

        public string StackTrace
        {
            get { return _stackTrace; }
            set { Set(nameof(StackTrace), ref _stackTrace, value); }
        }
    }
}
