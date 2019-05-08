using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace TomskASUProject.HallOfFame.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.IndexOf(".") + 1);
        public static IConfiguration Configuration { get; private set; } = GetConfiguration();

        public static int Main(string[] args)
        {
            Log.Logger = CreateSerilogLogger();
            var host = CreateWebHostBuilder(args).Build();

            try
            {
                Log.Information("Starting webhost {AppName}");

                bool.TryParse(Configuration["SeedData"], out var seed);
                if (seed)
                {
                    Log.Information("SeedData setting is true. Seeding...");
                    using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {
                        SeedData.EnsureSeedData(scope.ServiceProvider, "initdata.json");
                        Log.Information("Seeding successfully completed! ({ApplicationContext})", AppName);
                    }
                }

                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly {AppName}");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => 
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(Configuration)
                .UseSerilog();



        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            return config;
        }

        public static Serilog.ILogger CreateSerilogLogger()
        {

            var config = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithProperty("AppName", AppName)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(Configuration);


            return config.CreateLogger();
        }
    }
}
