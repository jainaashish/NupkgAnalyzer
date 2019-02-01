using NuGet.Protocol;
using NupkgAnalyzer.Common;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;

namespace NupkgAnalyzer.Commands
{
    public class MSBuildFilesCommand : ICommand
    {
        public Dictionary<string, string> Execute(ZipArchive archive, LocalPackageInfo localPackage)
        {
            var msbuildFiles = archive.Entries
                              .Where(e => e.FullName.EndsWith(localPackage.Identity.Id + ".targets", StringComparison.OrdinalIgnoreCase) ||
                                          e.FullName.EndsWith(localPackage.Identity.Id + ".props", StringComparison.OrdinalIgnoreCase));

            var results = new Dictionary<string, string>(); ;

            results.Add(Constants.MSBuildFiles, string.Join(";", msbuildFiles));
            return results;
        }
    }
}
