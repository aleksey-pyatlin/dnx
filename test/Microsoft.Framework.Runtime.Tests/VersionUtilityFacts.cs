using System;
using NuGet;
using Xunit;
using System.Runtime.Versioning;

namespace Microsoft.Framework.Runtime.Tests
{
    public class VersionUtilityFacts
    {
        [Theory]
        [InlineData("net45", "aspnet50", false)]
        [InlineData("aspnet50", "net45", true)]
        [InlineData("aspnetcore50", "k10", false)]
        [InlineData("k10", "aspnetcore50", false)]
        [InlineData("netcore45", "aspnetcore50", false)]
        [InlineData("win8", "aspnetcore50", false)]
        [InlineData("win81", "aspnetcore50", false)]
        [InlineData("aspnetcore50", "netcore45", false)]
        [InlineData("aspnetcore50", "win8", false)]
        [InlineData("aspnetcore50", "win81", false)]
        [InlineData("aspnetcore50", "portable-net40+win8+aspnetcore50", true)]
        [InlineData("net45", "aspnet452", false)]
        [InlineData("aspnet50", "net45", true)]
        [InlineData("aspnet50", "net45", true)]
        [InlineData("aspnetcore50", "k10", false)]
        [InlineData("k10", "aspnetcore50", false)]
        [InlineData("netcore45", "aspnetcore50", false)]
        [InlineData("win8", "aspnetcore50", false)]
        [InlineData("win81", "aspnetcore50", false)]
        [InlineData("aspnetcore50", "netcore45", false)]
        [InlineData("aspnetcore50", "win8", false)]
        [InlineData("aspnetcore50", "win81", false)]
        [InlineData("aspnetcore50", "portable-net40+win8+aspnetcore50", true)]
        // Temporary until our dependencies update
        [InlineData("aspnetcore50", "portable-net45+win8", true)]
        [InlineData("aspnetcore50", "portable-net451+win81", true)]
        [InlineData("aspnetcore50", "portable-net40+sl5+win8", false)]
        [InlineData("aspnetcore50", "portable-net45+win8", true)]
        [InlineData("aspnetcore50", "portable-net451+win81", true)]
        [InlineData("aspnetcore50", "portable-net40+sl5+win8", false)]

        // Tests for aspnet -> dnx rename
        [InlineData("dnx451", "aspnet50", true)]
        [InlineData("dnx452", "dnx451", true)]
        [InlineData("dnx451", "net45", true)]
        [InlineData("aspnet50", "dnx451", false)]
        [InlineData("net45", "dnx451", false)]

        [InlineData("dnxcore50", "aspnetcore50", true)]
        [InlineData("aspnetcore50", "dnxcore50", false)]
        // Portable stuff?

        [InlineData("dnxcore50", "portable-net40+win8+dnxcore50", true)]
        [InlineData("dnxcore50", "portable-net40+win8+aspnetcore50", true)]
        [InlineData("dnxcore50", "portable-net45+win8", true)]
        [InlineData("dnxcore50", "portable-net451+win81", true)]
        [InlineData("dnxcore50", "portable-net40+sl5+win8", false)]
        [InlineData("dnxcore50", "portable-net45+win8", true)]
        [InlineData("dnxcore50", "portable-net451+win81", true)]
        [InlineData("dnxcore50", "portable-net40+sl5+win8", false)]

        // Core50
        [InlineData("core50", "core50", true)]
        [InlineData("dnxcore50", "core50", true)]
        [InlineData("aspnetcore50", "core50", true)]
        [InlineData("dnx451", "core50", true)]
        [InlineData("dnx46", "core50", true)]
        [InlineData("net451", "core50", true)]
        [InlineData("net40", "core50", false)]
        [InlineData("sl20", "core50", false)]
        [InlineData("core50", "portable-net40+sl5+win8", false)]
        [InlineData("core50", "portable-net45+win8", true)]
        [InlineData("core50", "portable-net451+win81", true)]
        [InlineData("core50", "portable-net451+win8+core50", true)]
        [InlineData("core50", "portable-net451+win8+dnxcore50", true)]
        [InlineData("core50", "portable-net451+win8+aspnetcore50", true)]
        public void FrameworksAreCompatible(string project, string package, bool compatible)
        {
            var frameworkName1 = VersionUtility.ParseFrameworkName(project);
            var frameworkName2 = VersionUtility.ParseFrameworkName(package);

            var result = VersionUtility.IsCompatible(frameworkName1, frameworkName2);

            Assert.Equal(compatible, result);
        }

        [Theory]
        [InlineData(".NETPortable", "5.0", "core50")]
        [InlineData(".NETPortable", "5.1", "core51")]
        [InlineData(".NETPortable", "6.0", "core60")]
        public void ShortFrameworkNamesAreCorrect(string longName, string version, string shortName)
        {
            var fx = new FrameworkName(longName, Version.Parse(version));
            Assert.Equal(shortName, VersionUtility.GetShortFrameworkName(fx));
        }
    }
}
