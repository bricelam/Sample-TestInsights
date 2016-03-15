using System.Linq;
using Microsoft.Data.Sqlite;
using TestInsights.Data;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace TestInsights.Importer.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class ImporterTests
    {
        private static readonly string _testConnectionString = "" + new SqliteConnectionStringBuilder
        {
            DataSource = "ImporterTests.db"
        };

        [Fact]
        public virtual void Each_import_is_treated_as_new_test_run()
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var testImporter = new TestImporter(_testConnectionString);

            testImporter.Import("SampleRuns/InMemory.Tests.Output.xml");

            using (var context = CreateContext())
            {
                Assert.Equal(1, context.TestRuns.Count());
            }

            testImporter.Import("SampleRuns/InMemory.Tests.Output.xml");

            using (var context = CreateContext())
            {
                Assert.Equal(2, context.TestRuns.Count());
            }
        }

        [Fact]
        public virtual void Testrun_properties_are_populated()
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var testImporter = new TestImporter(_testConnectionString);

            testImporter.Import("SampleRuns/InMemory.Tests.Output.xml");

            using (var context = CreateContext())
            {
                var testRun = context.TestRuns.Include(tr => tr.TestResults).Single();
                Assert.Equal("2016-03-14 03:57:27", testRun.StartTime.ToString("yyyy-MM-dd hh:mm:ss"));
                Assert.Equal("64-bit .NET (unknown version) [collection-per-class, parallel (12 threads)]", testRun.TestEnvironment);
                Assert.Equal(39, testRun.TestResults.Count());
            }
        }

        [Fact]
        public virtual void Test_properties_are_populated()
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var testImporter = new TestImporter(_testConnectionString);

            testImporter.Import("SampleRuns/InMemory.Tests.Output.xml");

            using (var context = CreateContext())
            {
                var firstTest = context.Tests.Include(tr => tr.TestResults).Single(t => t.DisplayName == "Microsoft.EntityFrameworkCore.InMemory.Tests.InMemoryServiceCollectionExtensionsTest.Repeated_calls_to_add_do_not_modify_collection");
                Assert.Equal("Repeated_calls_to_add_do_not_modify_collection", firstTest.Method);
                Assert.Equal("Microsoft.EntityFrameworkCore.InMemory.Tests.InMemoryServiceCollectionExtensionsTest", firstTest.Class);
                Assert.Equal("Test collection for Microsoft.EntityFrameworkCore.InMemory.Tests.InMemoryServiceCollectionExtensionsTest", firstTest.Collection);
                Assert.Equal("Microsoft.EntityFrameworkCore.InMemory.Tests.dll", firstTest.Assembly);
                Assert.Equal(1, firstTest.TestResults.Count());
            }
        }

        [Fact]
        public virtual void Tests_are_unique()
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var testImporter = new TestImporter(_testConnectionString);

            testImporter.Import("SampleRuns/InMemory.Tests.Output.xml");

            using (var context = CreateContext())
            {
                var tests = context.Tests.Include(tr => tr.TestResults).Where(t => t.DisplayName == "Microsoft.EntityFrameworkCore.InMemory.Tests.InMemoryServiceCollectionExtensionsTest.Repeated_calls_to_add_do_not_modify_collection");
                Assert.Equal(1, tests.Count());
                Assert.Equal(1, tests.First().TestResults.Count());
            }

            testImporter.Import("SampleRuns/InMemory.Tests.Output.xml");

            using (var context = CreateContext())
            {
                var tests = context.Tests.Include(tr => tr.TestResults).Where(t => t.DisplayName == "Microsoft.EntityFrameworkCore.InMemory.Tests.InMemoryServiceCollectionExtensionsTest.Repeated_calls_to_add_do_not_modify_collection");
                Assert.Equal(1, tests.Count());
                Assert.Equal(2, tests.First().TestResults.Count());
            }
        }

        [Fact]
        public virtual void TestResults_are_stored()
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var testImporter = new TestImporter(_testConnectionString);

            testImporter.Import("SampleRuns/Microbenchmarks.Output.xml");

            using (var context = CreateContext())
            {
                Assert.Equal(61, context.TestResults.OfType<Data.TestPass>().Count());
                Assert.Equal(2, context.TestResults.OfType<Data.TestSkipped>().Count());
                Assert.Equal(2, context.TestResults.OfType<Data.TestFailed>().Count());
            }
        }

        [Fact]
        public virtual void Skipped_test_stores_reason()
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var testImporter = new TestImporter(_testConnectionString);

            testImporter.Import("SampleRuns/Microbenchmarks.Output.xml");

            using (var context = CreateContext())
            {
                var skippedTestResult = context.TestResults.OfType<Data.TestSkipped>().Include(tr => tr.Test).First();
                Assert.Equal("InitializeAndQuery_AdventureWorks [Variation: Warm (100 instances)]", skippedTestResult.Test.DisplayName);
                Assert.Equal("AdventureWorks2014 database does not exist on (localdb)\\mssqllocaldb. Download the AdventureWorks backup from https://msftdbprodsamples.codeplex.com/downloads/get/880661 and restore it to (localdb)\\mssqllocaldb to enable these tests.", skippedTestResult.Reason);
            }
        }

        [Fact]
        public virtual void Failed_test_stores_exception_type_and_stack_trace()
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var testImporter = new TestImporter(_testConnectionString);

            testImporter.Import("SampleRuns/Microbenchmarks.Output.xml");

            using (var context = CreateContext())
            {
                var failedTestResult = context.TestResults.OfType<Data.TestFailed>().Include(tr => tr.Test).First();
                Assert.Equal("Update [Variation: Batching Off]", failedTestResult.Test.DisplayName);
                Assert.Equal("System.NotSupportedException", failedTestResult.ExceptionType);
                Assert.Equal("System.NotSupportedException : The property 'CustomerId' on entity type 'Customer' is part of a key and so cannot be modified or marked as modified.", failedTestResult.Message);
                Assert.Equal(@"   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.InternalEntityEntry.SetPropertyModified(IProperty property, Boolean changeState, Boolean isModified) in D:\EntityFramework\src\Microsoft.EntityFrameworkCore\ChangeTracking\Internal\InternalEntityEntry.cs:line 201
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.ChangeDetector.DetectPropertyChanges(InternalEntityEntry entry) in D:\EntityFramework\src\Microsoft.EntityFrameworkCore\ChangeTracking\Internal\ChangeDetector.cs:line 94
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.ChangeDetector.DetectChanges(InternalEntityEntry entry) in D:\EntityFramework\src\Microsoft.EntityFrameworkCore\ChangeTracking\Internal\ChangeDetector.cs:line 76
   at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.ChangeDetector.DetectChanges(IStateManager stateManager) in D:\EntityFramework\src\Microsoft.EntityFrameworkCore\ChangeTracking\Internal\ChangeDetector.cs:line 70
   at Microsoft.EntityFrameworkCore.DbContext.TryDetectChanges(IStateManager stateManager) in D:\EntityFramework\src\Microsoft.EntityFrameworkCore\DbContext.cs:line 266
   at Microsoft.EntityFrameworkCore.DbContext.SaveChanges(Boolean acceptAllChangesOnSuccess) in D:\EntityFramework\src\Microsoft.EntityFrameworkCore\DbContext.cs:line 244
   at Microsoft.EntityFrameworkCore.DbContext.SaveChanges() in D:\EntityFramework\src\Microsoft.EntityFrameworkCore\DbContext.cs:line 222
   at Microsoft.EntityFrameworkCore.Microbenchmarks.UpdatePipeline.SimpleUpdatePipelineTests.Update(IMetricCollector collector, Boolean disableBatching) in D:\EntityFramework\test\Microsoft.EntityFrameworkCore.Microbenchmarks\UpdatePipeline\SimpleUpdatePipelineTests.cs:line 58", 
                    failedTestResult.StackTrace.Replace("\n", "\r\n"));
            }
        }

        private InsightContext CreateContext() => new InsightContext(_testConnectionString);

        public class TestImporter : XmlOutputImporter
        {
            public TestImporter(string connectionString)
                : base(connectionString)
            {
            }

            public override void LogOutput(string message)
            {
                base.LogOutput(message);
            }
        }
    }


}
