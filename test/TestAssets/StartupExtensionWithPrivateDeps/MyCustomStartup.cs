using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

[assembly: HostingStartup(typeof(StartupExtensionWithPrivateDeps.MyPrivateLoggerStartup))]

namespace StartupExtensionWithPrivateDeps
{
    internal class MyPrivateLoggerStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            ILogger logger = new PrivateLogger();
            logger.FakeMethod();
        }

        public Assembly GetLoggerAssembly()
        {
            return typeof(ILogger).Assembly;
        }

        private class PrivateLogger : ILogger
        {
            void ILogger.FakeMethod()
            { }
        }
    }
}
