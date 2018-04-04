using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

[assembly: HostingStartup(typeof(StartupExtensionWithWithUpgradedDeps.MyFileProviderStartup))]

namespace StartupExtensionWithWithUpgradedDeps
{
    internal class MyFileProviderStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            IFileProvider provider = new PrivateFileProvider();
            provider.FakeMethod();
        }

        public Assembly GetFileProviderAssembly()
        {
            return typeof(IFileProvider).Assembly;
        }

        private class PrivateFileProvider : IFileProvider
        {
            void IFileProvider.FakeMethod()
            { }
        }
    }
}
