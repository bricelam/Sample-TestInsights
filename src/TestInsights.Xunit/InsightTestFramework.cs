using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestInsights.Reporter
{
    // TODO: Allow wrapping arbitrary frameworks
    class InsightTestFramework : XunitTestFramework
    {
        public InsightTestFramework(IMessageSink messageSink)
            : base(new InsightMessageSink(messageSink))
        {
        }
    }
}
