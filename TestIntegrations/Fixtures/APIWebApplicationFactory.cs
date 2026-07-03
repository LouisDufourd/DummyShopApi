using DummyShopApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TestIntegrations.Fixtures
{
    public class APIWebApplicationFactory : WebApplicationFactory<Program>
    {
        public IConfiguration Configuration { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(AppContext.BaseDirectory);
            builder.UseEnvironment("Integrations");
            builder.ConfigureAppConfiguration(config =>
            {
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.Integrations.json")
                    .AddEnvironmentVariables()
                    .Build();

                config.AddConfiguration(Configuration);
            });
        }
    }
}
