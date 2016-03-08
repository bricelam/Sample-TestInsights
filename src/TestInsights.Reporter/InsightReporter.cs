using Xunit;
using Xunit.Abstractions;

namespace TestInsights.Reporter
{
    class InsightReporter : IRunnerReporter
    {
        public string Description => "Test Insights xUnit.net Reporter";
        public bool IsEnvironmentallyEnabled => true;
        public string RunnerSwitch => null;

        public IMessageSink CreateMessageHandler(IRunnerLogger logger)
            => new InsightReporterMessageHandler(logger);
    }
}
