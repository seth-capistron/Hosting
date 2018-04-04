using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

[assembly: HostingStartup(typeof(SimpleStartupExtension.MyCustomStartup))]

namespace SimpleStartupExtension
{
    internal class MyCustomStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.UseSetting("ExtensionName", nameof(MyCustomStartup));
        }

        public Assembly GetLoggerAssembly()
        {
            return typeof(ILogger).Assembly;
        }
    }
}
