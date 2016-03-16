using System;
using System.IO;
using System.Xml;
using TestInsights.Data;

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
            _db.Database.EnsureCreated();
        }

        public virtual void Import(string fileName)
        {
            using (var reader = XmlReader.Create(fileName, new XmlReaderSettings { IgnoreWhitespace = true }))
            {
                var assembly = String.Empty;
                var currentStartTime = default(DateTime);
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "assembly":
                                assembly = Path.GetFileNameWithoutExtension(reader["name"]);
                                currentStartTime = DateTime.Parse(reader["run-date"] + " " + reader["run-time"]);
                                break;

                            case "test":
                                var testName = reader["name"];
                                var type = reader["type"];
                                var time = reader["time"];
                                var testResult = ProcessTestResult(reader);
                                testResult.Assembly = assembly;
                                testResult.Class = type;
                                testResult.Name = testName;
                                testResult.StartTime = currentStartTime;

                                if (time != null)
                                {
                                    testResult.ExecutionTime = decimal.Parse(time);
                                }

                                _db.TestResults.Add(testResult);
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
