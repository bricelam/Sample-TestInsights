using System;
using System.IO;
using System.Xml;

namespace TestInsights.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                if (!File.Exists(arg))
                {
                    Console.WriteLine("File '" + arg + "' does not exist.");

                    continue;
                }

                Console.WriteLine("Importing '" + arg + "'...");

                using (var reader = XmlReader.Create(arg))
                {
                    // TODO: Process documents conforming to http://xunit.github.io/docs/format-xml-v2.html
                }
            }
        }
    }
}
