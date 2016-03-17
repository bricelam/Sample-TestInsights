using System;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using TestInsights.Data;
using TestInsights.Models;
using Xunit;
using Xunit.Abstractions;
using SdkTestMessageVisitor = Xunit.Sdk.TestMessageVisitor;

namespace TestInsights.Xunit
{
    public class InsightMessageSinkTests : IDisposable
    {
        private readonly string _connectionString = "" +
            new SqliteConnectionStringBuilder
            {
                DataSource = Path.GetRandomFileName()
            };
        private readonly InsightContext _db;

        public InsightMessageSinkTests()
        {
            _db = new InsightContext(_connectionString);
        }

        [Fact]
        public void Adds_passed_tests()
        {
            var sink = new InsightMessageSink(
                new SdkTestMessageVisitor(),
                _connectionString);

            var startTime = new DateTime(2015, 3, 16, 13, 23, 42);

            var testAssemblyStarting = new Mock<ITestAssemblyStarting>();
            testAssemblyStarting.Setup(m => m.StartTime).Returns(startTime);

            var assemblyInfo = new Mock<IAssemblyInfo>();
            assemblyInfo.Setup(m => m.Name).Returns("Assembly1");

            var testAssembly = new Mock<ITestAssembly>();
            testAssembly.Setup(m => m.Assembly).Returns(assemblyInfo.Object);

            var typeInfo = new Mock<ITypeInfo>();
            typeInfo.Setup(m => m.Name).Returns("Class1");

            var testClass = new Mock<ITestClass>();
            testClass.Setup(m => m.Class).Returns(typeInfo.Object);

            var test = new Mock<ITest>();
            test.Setup(m => m.DisplayName).Returns("Method1");

            var testPassed = new Mock<ITestPassed>();
            testPassed.Setup(m => m.TestAssembly).Returns(testAssembly.Object);
            testPassed.Setup(m => m.TestClass).Returns(testClass.Object);
            testPassed.Setup(m => m.Test).Returns(test.Object);
            testPassed.Setup(m => m.ExecutionTime).Returns(3.14m);

            sink.OnMessage(testAssemblyStarting.Object);
            sink.OnMessage(testPassed.Object);
            sink.OnMessage(Mock.Of<ITestAssemblyFinished>());

            var result = _db.Results.Include(r => r.Test.Class.Assembly).Cast<TestPassedResult>().Single();
            Assert.Equal("Assembly1", result.Test.Class.Assembly.Name);
            Assert.Equal("Class1", result.Test.Class.Name);
            Assert.Equal("Method1", result.Test.Name);
            Assert.Equal(startTime, result.StartTime);
            Assert.Equal(3.14m, result.ExecutionTime);
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Dispose();
        }
    }
}
