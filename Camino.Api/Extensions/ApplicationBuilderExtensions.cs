using Camino.Api.BackgroundJobs.Hangfire;
using Camino.Api.CustomMiddleware;
using Camino.Core.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Camino.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureRequestPipeline(this IApplicationBuilder application, IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
                application.UseSwagger();
                application.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BV HoaiDuc API v1");
                });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }
            application.UseCors("CorsPolicy");
            //application.UseHttpsRedirection();
            application.ConfigureCustomExceptionMiddleware();
            application.UseAuthentication();
            var jwtConfig = application.ApplicationServices.GetService(typeof(JwtConfig)) as JwtConfig;
            application.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter(jwtConfig?.SecretKey) }
            });
            HangfireScheduler.ConfigureRecurringJobs();
            application.UseMvc();
        }

        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder application)
        {
            application.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
