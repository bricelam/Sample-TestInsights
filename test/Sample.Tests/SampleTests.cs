using System;
using Xunit;

[assembly: TestFramework("TestInsights.Xunit.InsightTestFramework", "TestInsights.Xunit")]

namespace Sample.Tests
{
    public class SampleTests
    {
        private static readonly Random _random = new Random();

        [Fact]
        public void AlwaysPasses()
            => Assert.True(true);

        [Fact]
        public void SometimesFails()
            => Assert.NotEqual(0, _random.Next(3));

        [Fact]
        public void SometimesPasses()
            => Assert.Equal(0, _random.Next(3));

        [Fact]
        public void AlwaysFails()
            => Assert.True(false);

        [Fact(Skip = "Just because")]
        public void Skipped()
        {
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Theory(int i)
            => Assert.NotEqual(i, _random.Next(3));
    }
}
