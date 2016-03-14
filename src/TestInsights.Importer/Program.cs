using System;
using System.IO;
using System.Xml;
using Microsoft.Extensions.FileSystemGlobbing;

namespace TestInsights.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            var matcher = new Matcher();
            matcher.AddIncludePatterns(args);

            foreach (var file in matcher.GetResultsInFullPath(Directory.GetCurrentDirectory()))
            {
                Console.WriteLine("Importing '" + file + "'...");

                using (var reader = XmlReader.Create(file))
                {
                    // TODO: Process documents conforming to http://xunit.github.io/docs/format-xml-v2.html
                }
            }
        }
    }
}
