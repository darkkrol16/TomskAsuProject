using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TomskASUProject.HallOfFame.API;
using TomskASUProject.HallOfFame.API.Infrastructure.Contexts;
using Xunit;

namespace TomskAsuProject.HallOfFame.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var serviceProvider = new ServiceCollection()
                 .AddEntityFrameworkInMemoryDatabase()
                 .BuildServiceProvider();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAppDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });


                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopeServiceProvider = scope.ServiceProvider;

                    var context = scopeServiceProvider.GetRequiredService<ApplicationDbContext>();

                    context.Database.EnsureCreated();
                    if (!context.Persons.Any())
                    {
                        SeedData.SeedDataFromFile("initdata.json", context);
                    }
                }
            });
        }
    }
}
