using System.Collections.Generic;
using TestInsights.Data;

namespace TestInsights.Models
{
    public class Test
    {
        public string Assembly { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public ICollection<TestResult> TestResults { get; set; }
    }
}
