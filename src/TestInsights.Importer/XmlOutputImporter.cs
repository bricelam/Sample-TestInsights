using System;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using TestInsights.Data;

namespace TestInsights.Importer
{
    public class XmlOutputImporter
    {
        private readonly string _connectionString;

        public XmlOutputImporter()
        {
        }

        public XmlOutputImporter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual void Import(string fileName)
        {
            LogOutput("Importing '" + fileName + "'...");

            using (var reader = XmlReader.Create(fileName, new XmlReaderSettings { IgnoreWhitespace = true }))
            {
                using (var context = CreateContext())
                {
                    var assembly = "";
                    var collection = "";
                    TestRun currentTestRun = null;
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "assembly":
                                    assembly = reader["name"];
                                    currentTestRun = new TestRun
                                    {
                                        StartTime = DateTime.Parse(reader["run-date"] + " " + reader["run-time"]),
                                        TestEnvironment = reader["environment"]
                                    };
                                    context.TestRuns.Add(currentTestRun);
                                    break;
                                case "collection":
                                    collection = reader["name"];
                                    break;
                                case "test":
                                    var testName = reader["name"];
                                    var test = context.Find<Test>(t => t.DisplayName == testName);
                                    if (test == null)
                                    {
                                        test = new Test
                                        {
                                            Assembly = assembly,
                                            Class = reader["type"],
                                            Collection = collection,
                                            DisplayName = testName,
                                            Method = reader["method"]
                                        };
                                        context.Tests.Add(test);
                                    }
                                    var executionTime = reader["time"] ?? "0";
                                    var testResult = ProcessTestResult(reader);
                                    testResult.Test = test;
                                    testResult.TestRun = currentTestRun;
                                    testResult.ExecutionTime = decimal.Parse(executionTime);
                                    context.TestResults.Add(testResult);
                                    break;
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }

            LogOutput("Import finished.");
        }

        public virtual InsightContext CreateContext() => _connectionString != null ? new InsightContext(_connectionString) : new InsightContext();

        private TestResult ProcessTestResult(XmlReader reader)
        {
            var result = reader["result"];
            switch (result)
            {
                case "Pass":
                    return new TestPass();
                case "Fail":
                    var testFailed = new TestFailed();
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "failure")
                            {
                                testFailed.ExceptionType = reader["exception-type"];
                                continue;
                            }
                            if (reader.Name == "message")
                            {
                                testFailed.Message = reader.ReadElementContentAsString();
                            }
                            if (reader.Name == "stack-trace")
                            {
                                testFailed.StackTrace = reader.ReadElementContentAsString();
                                break;
                            }
                        }
                    }
                    return testFailed;
                case "Skip":
                    var testSkipped = new TestSkipped();
                    if (reader.ReadToDescendant("reason"))
                    {
                        testSkipped.Reason = reader.ReadElementContentAsString();
                    }
                    return testSkipped;
            }
            return null;
        }

        public virtual void LogOutput(string message)
        {
            //Console.WriteLine(message);
        }
    }
}
