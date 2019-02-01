using NuGet.Protocol;
using NupkgAnalyzer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NupkgAnalyzer.Commands
{
    public class InteropAssembliesCommand : ICommand
    {
        private string _tempeExtractionPath;
        public InteropAssembliesCommand(string tempExtractionPath)
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
                    .Where(e => e.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

            var interopAssemblies = new List<string>();

            if (ps1Files.Count() > 1)
            {
                bool? isInteropType = null;
                foreach (var scriptFile in ps1Files)
                {
                    var path = GetRandomPath();
                    scriptFile.ExtractToFile(path, true);

                    try
                    {
                        Assembly asembly = Assembly.LoadFrom(path);

                        var isCurrentInterop = asembly.CustomAttributes.Any(attribute => attribute.AttributeType.Name.Equals("ImportedFromTypeLibAttribute", StringComparison.OrdinalIgnoreCase));


                        if (isInteropType == null)
                        {
                            isInteropType = isCurrentInterop;
                        }
                        else if (isCurrentInterop != isInteropType)
                        {
                            interopAssemblies.Add(scriptFile.FullName);
                        }
                    }
                    catch (Exception) { }
                }
            }

            results.Add(Constants.InteropAssemblies, string.Join(";", interopAssemblies));
            return results;
        }
    }
}
