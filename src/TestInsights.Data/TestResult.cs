namespace TestInsights.Data
{
    public abstract class TestResult
    {
        public int Id { get; set; }
        public decimal ExecutionTime { get; set; }
        public Test Test { get; set; }
        public TestRun TestRun { get; set; }
    }
}
