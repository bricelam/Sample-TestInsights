using Xunit;

namespace TestInsights.Reporter
{
    class InsightReporterMessageHandler : TestMessageVisitor
    {
        public InsightReporterMessageHandler(IRunnerLogger logger)
        {
        }
    }
}