using System.IO;
using Camino.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace Camino.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var nLogConfigPath = string.Concat(Directory.GetCurrentDirectory(), "/Nlog.config");
            if (File.Exists(nLogConfigPath)) { LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config")); }
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureApplicationServices(_configuration, _hostingEnvironment);
        }

        public void Configure(IApplicationBuilder application, IHostingEnvironment hostingEnvironment)
        {
            application.ConfigureRequestPipeline(hostingEnvironment);
        }
    }
}
