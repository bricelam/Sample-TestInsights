using System;
using System.IO;
using Microsoft.Extensions.FileSystemGlobbing;

namespace TestInsights.Importer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var matcher = new Matcher();
            matcher.AddIncludePatterns(args);

            var importer = new XmlOutputImporter();

            foreach (var file in matcher.GetResultsInFullPath(Directory.GetCurrentDirectory()))
            {
                Console.WriteLine("Importing '" + file + "'...");

                importer.Import(file);
            }

            Console.WriteLine("Done.");
        }
    }
}
