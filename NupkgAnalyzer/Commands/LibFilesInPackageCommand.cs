using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using NuGet.Protocol;
using System.Linq;
using System.Text.RegularExpressions;
using NupkgAnalyzer.Common;

namespace NupkgAnalyzer.Commands
{
    public class LibFilesInPackageCommand : ICommand
    {
        public Dictionary<string, string> Execute(ZipArchive archive, LocalPackageInfo localPackage)
        {
            var regex = new Regex(@"(lib)\/[^\/]+\.(dll)", RegexOptions.IgnoreCase);

            var LibsWithoutTFM = archive.Entries
                              .Where(e => regex.IsMatch(e.FullName));

            var results = new Dictionary<string, string>(); ;

            results.Add(Constants.LibWithoutTFM, string.Join(";", LibsWithoutTFM));
            return results;
        }
    }
}
