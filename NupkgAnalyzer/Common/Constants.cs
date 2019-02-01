using System;
using System.Collections.Generic;
using System.Text;

namespace NupkgAnalyzer.Common
{
    public class Constants
    {
        public static string PackageId = nameof(PackageId);
        public static string Version = nameof(Version);
        public static string InteropAssemblies = nameof(InteropAssemblies);
        public static string ContentFiles = nameof(ContentFiles);
        public static string LibWithoutTFM = nameof(LibWithoutTFM);
        public static string PS1Scripts = nameof(PS1Scripts);
        public static string PS1ScriptsWithNuGetAPIs = nameof(PS1ScriptsWithNuGetAPIs);
        public static string MSBuildFiles = nameof(MSBuildFiles);
        public static string XdtFiles = nameof(XdtFiles);
    }
}
