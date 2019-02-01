using NuGet.Common;
using NuGet.Protocol;
using NupkgAnalyzer.Commands;
using NupkgAnalyzer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace NupkgAnallyzer
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                throw new ArgumentException("Must pass nupkgPath");
            }

            var nupkgsPath = args[0];
            var outputPath = Environment.CurrentDirectory;

            if (!string.IsNullOrEmpty(args[1]))
            {
                outputPath = args[1];
            }

            var results = ExecuteCommand(new InteropAssembliesCommand(outputPath), nupkgsPath);

            var values = new List<List<string>>();

            var names = new List<string>() { Constants.PackageId, Constants.Version, Constants.InteropAssemblies };

            values.Add(names);
            foreach (var dict in results)
            {
                var row = new List<string>();
                foreach (var key in names)
                {
                    string value;
                    dict.TryGetValue(key, out value);
                    row.Add(value);
                }
                values.Add(row);
            }

            var csv = new StringBuilder();

            foreach (var row in values)
            {
                csv.Append(string.Join(",", row)).Append("\r\n");
            }

            File.WriteAllText(Path.Combine(outputPath, "PackagesWithInteropIssue.csv"), csv.ToString());
        }

        private static IEnumerable<LocalPackageInfo> GetAllPackagesInFolder(string rootPath)
        {
            return LocalFolderUtility.GetPackagesV3(root: rootPath, log: NullLogger.Instance);
        }

        private static List<Dictionary<string, string>> ExecuteCommand(ICommand command, string nupkgsRoot)
        {
            var result = new List<Dictionary<string, string>>();

            foreach (var package in GetAllPackagesInFolder(nupkgsRoot))
            {
                var results = new Dictionary<string, string>();
                results.Add(Constants.PackageId, package.Identity.Id);
                results.Add(Constants.Version, package.Identity.Version.ToFullString());

                using (var archive = ZipFile.Open(package.Path, ZipArchiveMode.Read))
                {
                    var commandResult = command.Execute(archive, package);
                    commandResult.ToList().ForEach(e => results.Add(e.Key, e.Value));
                }

                result.Add(results);

            }

            return result;
        }
    }
}
