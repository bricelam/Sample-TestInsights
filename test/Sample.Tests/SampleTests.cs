using Xunit;

[assembly: TestFramework("TestInsights.Reporter.InsightTestFramework", "TestInsights.Xunit")]

namespace Sample.Tests
{
    public class SampleTests
    {
        [Fact]
        public void AlwaysPasses()
        {
            Assert.True(true);
        }
    }
}
