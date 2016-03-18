Test Insights
=============
See beyond red and green.

Capturing Results
-----------------
There are two ways that Test Insights can capture test results.

### Importer

The importer allow you to import a standard xUnit.net XML file.

    xunit.console MyTests.dll -xml results.xml
    TestInsights.Importer results.xml

### Test Framework

You can also use the Test Insights xUnit.net Test Framework to capture the results of tests as they're run inside Visual
Studio and elsewhere.

```C#
[assembly: TestFramework(
    "TestInsights.Xunit.InsightTestFramework",
    "TestInsights.Xunit")]
```

Viewing Results
---------------
The main application allows you to view the captured results.
