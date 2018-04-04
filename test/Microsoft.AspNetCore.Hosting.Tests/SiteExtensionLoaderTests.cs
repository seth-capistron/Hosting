using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.AspNetCore.Hosting.Tests
{
#if !NET461
    public class SiteExtensionLoaderTests
    {
        [Fact]
        public void ItLoadsSimpleSiteExtensionWithoutDependencies()
        {
            var extPath = GetSiteExtensionPath("SimpleStartupExtension");
            var loader = new SiteExtensionLoader(extPath);
            var builder = new WebHostBuilder();
            var startup = Assert.Single(loader.GetStartups());

            // Assert the extension can execute
            startup.Configure(builder);
            Assert.Equal("MyCustomStartup", builder.GetSetting("ExtensionName"));

            // Assert it unifies types with the default load context
            var loggerGetter = startup.GetType().GetMethod("GetLoggerAssembly");
            var loggerAssembly = (Assembly)loggerGetter.Invoke(startup, Array.Empty<object>());
            Assert.Same(typeof(ILogger).Assembly, loggerAssembly);
        }

        [Fact]
        public void ItLoadsExtensionWithDependencies()
        {
            var extPath = GetSiteExtensionPath("StartupExtensionWithDeps");
            var loader = new SiteExtensionLoader(extPath);
            var builder = new WebHostBuilder();
            var startup = Assert.Single(loader.GetStartups());

            // Assert the extension can execute code from a dependency
            startup.Configure(builder);
            Assert.Equal("Yellow", builder.GetSetting("ExtensionColor"));

            // Assert it loaded from the directory next to StartupExtensionWithDeps.dll
            var class1Getter = startup.GetType().GetMethod("GetClass1");
            var class1Assembly = (Assembly)class1Getter.Invoke(startup, Array.Empty<object>());
            Assert.Equal(Path.GetDirectoryName(extPath), Path.GetDirectoryName(class1Assembly.Location));
        }

        [Fact]
        public void ItLoadsExtensionsWithNativeDeps()
        {
            var extPath = GetSiteExtensionPath("StartupExtensionWithSqlite");
            var loader = new SiteExtensionLoader(extPath);
            var builder = new WebHostBuilder();
            var startup = Assert.Single(loader.GetStartups());

            // Assert the extension can execute code from a dependency
            startup.Configure(builder);
            Assert.Equal("3.13.0", builder.GetSetting("SqliteVersion"));
        }

        [Fact]
        public void SiteExtensionsCanDeclarePrivateAssemblies()
        {
            var extPath = GetSiteExtensionPath("StartupExtensionWithPrivateDeps");
            var loader = new SiteExtensionLoader(extPath);
            var builder = new WebHostBuilder();
            var startup = Assert.Single(loader.GetStartups());

            var getter = startup.GetType().GetMethod("GetLoggerAssembly");
            var startupLoggerAssembly = (Assembly)getter.Invoke(startup, Array.Empty<object>());
            var startupAssemblyName = startupLoggerAssembly.GetName();

            var hostLoggerAssembly = typeof(ILogger).Assembly;
            var hostAssemblyName = hostLoggerAssembly.GetName();
            
            // Assert it can execute
            startup.Configure(builder);

            Assert.NotSame(typeof(ILogger).Assembly, startupLoggerAssembly);
            Assert.NotEqual(hostAssemblyName.Version, startupAssemblyName.Version);
            Assert.Equal(hostAssemblyName.Name, startupAssemblyName.Name);
            Assert.Equal(hostAssemblyName.KeyPair, startupAssemblyName.KeyPair);
            Assert.Equal(hostAssemblyName.CultureName, startupAssemblyName.CultureName);
        }

        private string GetSiteExtensionPath(string name)
        {
            var mainAssembly = typeof(SiteExtensionLoaderTests).Assembly
                .GetCustomAttributes<SiteExtensionReferenceAttribute>()
                .First(f => string.Equals(f.Name, name, StringComparison.Ordinal))
                .FilePath;
            return Path.Combine(Path.GetDirectoryName(mainAssembly), "siteextension.config");
        }
    }
#endif
}
