using System.Collections.Generic;

namespace TestInsights.Data
{
    public class Test
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Method { get; set; }
        public string Class { get; set; }
        public string Collection { get; set; }
        public string Assembly { get; set; }
        public ICollection<TestResult> TestResults { get; } = new List<TestResult>();
    }
}
