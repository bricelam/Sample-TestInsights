using System;
using Microsoft.EntityFrameworkCore;
using TestInsights.Data;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestInsights.Xunit
{
    class InsightMessageSink : TestMessageVisitor
    {
        private static volatile bool _migrated;
        private static object _migrateSync = new object();

        private readonly IMessageSink _messageSink;
        private readonly InsightContext _db;

        private DateTime _currentStartTime;

        public InsightMessageSink(IMessageSink messageSink)
        {
            _messageSink = messageSink;
            _db = new InsightContext();
        }

        public InsightMessageSink(IMessageSink messageSink, string connectionString)
        {
            _messageSink = messageSink;
            _db = new InsightContext(connectionString);
        }

        public override bool OnMessage(IMessageSinkMessage message)
        {
            base.OnMessage(message);

            return _messageSink.OnMessage(message);
        }

        protected override bool Visit(ITestAssemblyStarting assemblyStarting)
        {
            EnsureMigrated();

            _currentStartTime = assemblyStarting.StartTime;

            return true;
        }

        protected override bool Visit(ITestSkipped testSkipped)
            => Add(testSkipped, new TestSkippedResult { Reason = testSkipped.Reason });

        protected override bool Visit(ITestPassed testPassed)
            => Add(testPassed, new TestPassedResult());

        protected override bool Visit(ITestFailed testFailed)
            => Add(
                testFailed,
                new TestFailedResult
                {
                    ExceptionType = testFailed.ExceptionTypes[0],
                    Message = testFailed.Messages[0],
                    StackTrace = testFailed.StackTraces[0]
                });

        protected override bool Visit(ITestAssemblyFinished assemblyFinished)
        {
            _db.SaveChanges();

            return true;
        }

        private void EnsureMigrated()
        {
            if (!_migrated)
            {
                lock (_migrateSync)
                {
                    if (!_migrated)
                    {
                        _db.Database.Migrate();
                        _migrated = true;
                    }
                }
            }
        }

        private bool Add(ITestResultMessage message, TestResult testResult)
        {
            testResult.Assembly = message.TestAssembly.Assembly.Name;
            testResult.Class = message.TestClass.Class.Name;
            testResult.Name = message.Test.DisplayName;
            testResult.StartTime = _currentStartTime;
            testResult.ExecutionTime = message.ExecutionTime;

            _db.Add(testResult);

            return true;
        }
    }
}