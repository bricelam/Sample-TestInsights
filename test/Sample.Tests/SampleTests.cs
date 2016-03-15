using System;
using Xunit;

[assembly: TestFramework("TestInsights.Xunit.InsightTestFramework", "TestInsights.Xunit")]

namespace Sample.Tests
{
    public class SampleTests
    {
        private static readonly Random _rand = new Random();

        [Fact]
        public void AlwaysPasses()
        {
            Assert.True(true);
        }

        [Fact]
        public void SometimesFails()
        {
            Assert.NotEqual(0, _rand.Next(3));
        }

        [Fact]
        public void SometimesPasses()
        {
            Assert.Equal(0, _rand.Next(3));
        }

        [Fact]
        public void AlwaysFails()
        {
            Assert.True(false);
        }

        [Fact(Skip = "Just because")]
        public void Skipped()
        {
        }
    }
}
