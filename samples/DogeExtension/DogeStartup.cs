using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(SampleStartupExtension.DogeStartup))]

namespace SampleStartupExtension
{
    // redirects all requests to a picture of doge
    public class DogeStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            // such startup!
            builder.ConfigureServices(services =>
                            services.AddSingleton<IStartupFilter, DogeStartupFilter>());
        }

        private class DogeStartupFilter : IStartupFilter
        {
            // much configure
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return app =>
                {
                    app.Use(async (ctx, n) =>
                    {
                        // Wow rewrite path!
                        ctx.Request.Path = "/doge.jpg";
                        await n();
                    });

                    next(app);
                };
            }
        }
    }
}
