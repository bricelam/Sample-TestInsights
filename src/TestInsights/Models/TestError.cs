using System;
using GalaSoft.MvvmLight;

namespace TestInsights.Models
{
    public class TestError : ObservableObject
    {
        private DateTime _startDate;
        private string _error;
        private string _message;
        private string _stackTrace;

        public DateTime StartDate
        {
            get { return _startDate; }
            set { Set(nameof(StartDate), ref _startDate, value); }
        }

        public string Error
        {
            get { return _error; }
            set { Set(nameof(Error), ref _error, value); }
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
