using System;
using System.IO;
using System.Xml;
using TestInsights.Data;
using TestInsights.Models;

namespace TestInsights.Importer
{
    internal class XmlOutputImporter
    {
        private readonly InsightContext _db;

        public XmlOutputImporter()
        {
            _db = new InsightContext();
        }

        public XmlOutputImporter(string connectionString)
        {
            _db = new InsightContext(connectionString);
        }

        public virtual void Import(string fileName)
        {
            _db.Database.EnsureCreated();

            using (var reader = XmlReader.Create(fileName, new XmlReaderSettings { IgnoreWhitespace = true }))
            {
                var assemblyName = String.Empty;
                var currentStartTime = default(DateTime);
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "assembly":
                                assemblyName = Path.GetFileNameWithoutExtension(reader["name"]);
                                currentStartTime = DateTime.Parse(reader["run-date"] + " " + reader["run-time"]);
                                break;

                            case "test":
                                var name = reader["name"];
                                var className = reader["type"];
                                if (name.StartsWith(className + "."))
                                {
                                    name = name.Substring(className.Length + 1);
                                }
                                var time = reader["time"];
                                var assembly = _db.Find<TestAssembly>(a => a.Name == assemblyName)
                                    ?? _db.Add(new TestAssembly { Name = assemblyName }).Entity;
                                var testClass = _db.Find<TestClass>(c => c.Assembly == assembly && c.Name == className)
                                    ?? _db.Add(new TestClass { Assembly = assembly, Name = className }).Entity;
                                var test = _db.Find<Test>(t => t.Class == testClass && t.Name == name)
                                    ?? _db.Add(new Test { Class = testClass, Name = name }).Entity;
                                var testResult = ProcessTestResult(reader);
                                testResult.Test = test;
                                testResult.StartTime = currentStartTime;

                                if (time != null)
                                {
                                    testResult.ExecutionTime = decimal.Parse(time);
                                }

                                _db.Results.Add(testResult);
                                break;
                        }
                    }
                }
                _db.SaveChanges();
            }
        }

        private TestResult ProcessTestResult(XmlReader reader)
        {
            var result = reader["result"];
            switch (result)
            {
                case "Pass":
                    return new TestPassedResult();
                case "Fail":
                    var testFailed = new TestFailedResult();
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
                    var testSkipped = new TestSkippedResult();
                    if (reader.ReadToDescendant("reason"))
                    {
                        testSkipped.Reason = reader.ReadElementContentAsString();
                    }
                    return testSkipped;
            }
            return null;
        }
    }
}
