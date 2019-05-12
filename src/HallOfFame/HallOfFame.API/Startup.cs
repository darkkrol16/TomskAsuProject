using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using TomskASUProject.HallOfFame.API.Infrastructure.Contexts;
using TomskASUProject.HallOfFame.API.Infrastructure.Repositories;

namespace TomskASUProject.HallOfFame.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomMvc()
                .AddCustomDbContext(Configuration)
                .AddSwagger()
                .AddApiVersioning()
                .AddVersionedApiExplorer(o => {
                    o.GroupNameFormat = "'v'V";
                    o.SubstituteApiVersionInUrl = true;
                })
                .AddTransient<IPersonRepository, PersonRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider apiProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                foreach (var description in apiProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Hall of fame API {description.GroupName}");
                }
            });


            //Making redirect from index page to swagger page
            var rewriter = new RewriteOptions();
            rewriter.AddRedirect("^$", "swagger");
            app.UseRewriter(rewriter);


            app.UseMvcWithDefaultRoute();
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(builder => {
                builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                options => {
                    options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options => {
                var apiProvider = services.BuildServiceProvider()
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in apiProvider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new Info
                    {
                        Title = $"Hall of fame API",
                        Version = $"{description.GroupName}",
                    });
                }
            });
            return services;
        }
    }
}
