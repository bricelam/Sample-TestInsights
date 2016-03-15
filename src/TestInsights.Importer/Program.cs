using System.IO;
using Microsoft.Extensions.FileSystemGlobbing;

namespace TestInsights.Importer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var matcher = new Matcher();
            matcher.AddIncludePatterns(args);
            // TODO: allow user to specify connection string through args
            var importer = new XmlOutputImporter();

            foreach (var file in matcher.GetResultsInFullPath(Directory.GetCurrentDirectory()))
            {
                importer.Import(file);
            }
        }
    }
}
