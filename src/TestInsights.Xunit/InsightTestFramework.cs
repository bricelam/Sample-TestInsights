using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestInsights.Xunit
{
    // TODO: Allow wrapping arbitrary frameworks
    public class InsightTestFramework : XunitTestFramework
    {
        public InsightTestFramework(IMessageSink messageSink)
            : base(new InsightMessageSink(messageSink))
        {
        }
    }
}
