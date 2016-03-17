using System;

namespace TestInsights.Models
{
    public abstract class TestResult
    {
        public Test Test { get; set; }
        public DateTime StartTime { get; set; }
        public decimal ExecutionTime { get; set; }
    }
}
