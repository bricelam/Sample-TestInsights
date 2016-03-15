using Microsoft.EntityFrameworkCore;
using TestInsights.Data;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestInsights.Xunit
{
    class InsightMessageSink : TestMessageVisitor
    {
        private readonly IMessageSink _messageSink;
        private readonly InsightContext _db = new InsightContext();

        private TestRun _currentRun;

        public InsightMessageSink(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public override bool OnMessage(IMessageSinkMessage message)
        {
            base.OnMessage(message);

            return _messageSink.OnMessage(message);
        }

        protected override bool Visit(ITestAssemblyStarting assemblyStarting)
        {
            _db.Database.Migrate();

            _currentRun = new TestRun
            {
                StartTime = assemblyStarting.StartTime,
                TestEnvironment = assemblyStarting.TestEnvironment
            };

            return true;
        }

        protected override bool Visit(ITestAssemblyFinished assemblyFinished)
        {
            _currentRun = null;

            return true;
        }

        protected override bool Visit(ITestSkipped testSkipped)
            => Add(testSkipped, new Data.TestSkipped { Reason = testSkipped.Reason });

        protected override bool Visit(ITestPassed testPassed)
            => Add(testPassed, new TestPass());

        protected override bool Visit(ITestFailed testFailed)
            => Add(
                testFailed,
                new Data.TestFailed
                {
                    ExceptionType = testFailed.ExceptionTypes[0],
                    Message = testFailed.Messages[0],
                    StackTrace = testFailed.StackTraces[0]
                });

        private bool Add(ITestResultMessage message, TestResult testResult)
        {
            testResult.ExecutionTime = message.ExecutionTime;
            testResult.TestRun = _currentRun;
            testResult.Test = _db.Find<Test>(t => t.DisplayName == message.Test.DisplayName)
                ?? new Test
                {
                    Assembly = message.TestAssembly.Assembly.Name,
                    Collection = message.TestCollection.DisplayName,
                    Class = message.TestClass.Class.Name,
                    Method = message.TestMethod.Method.Name,
                    DisplayName = message.TestCase.DisplayName
                };

            _db.Add(testResult);
            _db.SaveChanges();

            return true;
        }
    }
}