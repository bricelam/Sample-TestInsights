using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestInsights.Xunit
{
    class InsightTestFrameworkExecutor : XunitTestFrameworkExecutor
    {
        public InsightTestFrameworkExecutor(
            AssemblyName assemblyName,
            ISourceInformationProvider sourceInformationProvider,
            IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
        }

        protected override void RunTestCases(
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
            => base.RunTestCases(testCases, new InsightMessageSink(executionMessageSink), executionOptions);
    }
}
