using System;
using System.Collections.Generic;

namespace TestInsights.Data
{
    public class TestRun
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public string TestEnvironment { get; set; }
        public ICollection<TestResult> TestResults { get; } = new List<TestResult>();
    }
}
