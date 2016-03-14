using Xunit.Abstractions;

namespace TestInsights.Reporter
{
    class InsightMessageSink : IMessageSink
    {
        private readonly IMessageSink _messageSink;

        public InsightMessageSink(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public bool OnMessage(IMessageSinkMessage message)
        {
            // TODO: Process
            return _messageSink.OnMessage(message);
        }
    }
}