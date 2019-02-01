using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NuGet.Protocol;
using NupkgAnalyzer.Common;

namespace NupkgAnalyzer.Commands
{
    public class PSScriptsCommand : ICommand
    {
        private string _tempeExtractionPath;
        public PSScriptsCommand(string tempExtractionPath)
        {
            _tempeExtractionPath = tempExtractionPath;
        }

        private string GetRandomPath()
        {
            return Path.Combine(_tempeExtractionPath, Guid.NewGuid().ToString());
        }

        public Dictionary<string, string> Execute(ZipArchive archive, LocalPackageInfo localPackage)
        {
            var results = new Dictionary<string, string>(); ;

            var ps1Files = archive.Entries
                    .Where(e => e.FullName.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
                    .ToArray();


            if (ps1Files.Count() > 0)
            {
                var ps1FilesContainingNuGetAPIs = new List<string>();

                foreach (var scriptFile in ps1Files)
                {
                    var path = GetRandomPath();
                    scriptFile.ExtractToFile(path, true);

                    using (var stream = scriptFile.Open())
                    using (var reader = new StreamReader(stream))
                    {
                        var scriptFileContent = reader.ReadToEnd();
                        if (scriptFileContent.Contains("NuGet.VisualStudio.IFileSystemProvider") ||
                            scriptFileContent.Contains("NuGet.VisualStudio.ISolutionManager"))
                        {
                            ps1FilesContainingNuGetAPIs.Add(scriptFile.FullName);
                        }
                    }
                }

                results.Add(Constants.PS1ScriptsWithNuGetAPIs, string.Join(";", ps1FilesContainingNuGetAPIs));
            }

            results.Add(Constants.PS1Scripts, string.Join(";", ps1Files.Select(ps => ps.FullName)));

            return results;
        }

    }
}
