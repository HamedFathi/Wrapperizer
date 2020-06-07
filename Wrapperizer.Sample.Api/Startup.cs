using System;
using Funx.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wrapperizer.Abstraction;
using Wrapperizer.Abstraction.Specifications;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Sample.Api.Queries;

namespace Wrapperizer.Sample.Api
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
            services.AddControllers();

            services.AddEntityFrameworkInMemoryDatabase();

            services.AddDistributedMemoryCache();
            // services.AddStackExchangeRedisCache(options =>
            // {
            //     options.Configuration = "localhost";
            //     options.InstanceName = "Wrapperizer.Api";
            // });

            services.AddWrapperizer()
                .AddHandlers(configure: context => context
                        .AddGlobalValidation()
                        .AddDistributedCaching()
                )
                .AddCrudRepositories<WeatherForecastDbContext>((provider, options) =>
                {
                    options.UseInMemoryDatabase("WeatherForecast");
                    options.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
                });

            services.AddTransient<Specification<GetWeatherForecast>, NotPastSpecification>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            SeedDatabase(app.ApplicationServices.CreateScope().ServiceProvider
                .GetRequiredService<ICrudRepository<WeatherForecast>>());
        }

        private static void SeedDatabase(ICrudRepository<WeatherForecast> repository)
        {
            var rng = new Random();
            new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            }.ForEach((summary, index) =>
            {
                repository.Add(new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = summary
                });
            });
        }
    }
}
