using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using Microsoft.Extensions.Logging;
namespace HelloNancy
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, ILoggerFactory LoggerFactory)
        {
            app.UseOwin(x => x.UseNancy());
            LoggerFactory.AddConsole();
        }
    }
}