using System;

namespace TestInsights.Data
{
    public abstract class TestResult
    {
        public string Assembly { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public decimal ExecutionTime { get; set; }
    }
}
