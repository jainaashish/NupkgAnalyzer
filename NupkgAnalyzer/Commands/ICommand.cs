using NuGet.Protocol;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace NupkgAnalyzer.Commands
{
    public interface  ICommand
    {
        Dictionary<string, string> Execute(ZipArchive archive, LocalPackageInfo localPackage);
    }
}
