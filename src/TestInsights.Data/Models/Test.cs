using System.Collections.Generic;

namespace TestInsights.Models
{
    public class Test
    {
        public string Assembly { get; set; }
        public string Class { get; set; }
        public string Name { get; set; }
        public ICollection<TestResult> Results { get; } = new List<TestResult>();
    }
}
