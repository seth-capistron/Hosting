using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DogeApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSetting(WebHostDefaults.HostingStartupExtensionsKey, @"..\DogeExtension\bin\Debug\netstandard2.0\startupextension.config")
                .UseStartup<Startup>();
    }
}
