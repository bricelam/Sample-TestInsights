using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestInsights.Xunit
{
    // TODO: Allow wrapping arbitrary frameworks
    class InsightTestFramework : XunitTestFramework
    {
        public InsightTestFramework(IMessageSink messageSink)
            : base(messageSink)
        {
        }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
            => new InsightTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }
}
