using System.Reflection;
using ClassLib1;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(StartupExtensionWithDeps.MyStartupWithDeps))]

namespace StartupExtensionWithDeps
{
    internal class MyStartupWithDeps : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.UseSetting("ExtensionColor", new Class1().GetColor());
        }

        public Assembly GetClass1()
        {
            return typeof(Class1).Assembly;
        }
    }
}
