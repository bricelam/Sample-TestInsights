using Xunit;
using Xunit.Abstractions;

namespace TestInsights.Xunit
{
    class InsightReporter : IRunnerReporter
    {
        public string Description
            => "Test Insights xUnit.net Reporter";

        public bool IsEnvironmentallyEnabled
            => false;

        public string RunnerSwitch
            => "insight";

        public IMessageSink CreateMessageHandler(IRunnerLogger logger)
            => new InsightMessageSink(new DefaultRunnerReporterMessageHandler(logger));
    }
}
