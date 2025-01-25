using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Infrastructure;
using Camino.Services.InitialData;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Camino.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            //var host = CreateWebHostBuilder(args).Build();
            //OnApplicationStart(host);
            //host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .UseIISIntegration();
                //.UseKestrel(o => { o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10); });

        private static void OnApplicationStart(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var initService = services.GetRequiredService<IInitialService>();
                    initService.DummyData();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILoggerManager>();
                    logger.LogError($"An error occurred while seeding the database: {ex}");
                }
            }
        }
    }
}
