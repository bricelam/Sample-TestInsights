using System;
using GalaSoft.MvvmLight;

namespace TestInsights.Models
{
    public abstract class TestResult : ObservableObject
    {
        private Test _test;
        private DateTime _startTime;
        private decimal _executionTime;

        public Test Test
        {
            get { return _test; }
            set { Set(nameof(Test), ref _test, value); }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set { Set(nameof(StartTime), ref _startTime, value); }
        }

        public decimal ExecutionTime
        {
            get { return _executionTime; }
            set { Set(nameof(ExecutionTime), ref _executionTime, value); }
        }
    }
}
