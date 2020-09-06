using Auth.Api.Configuration;
using Auth.Api.Middlewares;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mist.Auth.Application.AutoMapper;
using Mist.Auth.Infra;

namespace Auth.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostEnvironment hostEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddJwtConfiguration(Configuration);
            services.WebApiConfig();
            services.ResolveDependencies(Configuration);

            services.AddMediatR(typeof(Startup));
            services.AddAutoMapper(typeof(ViewModelToDomainMappingProfile));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMvcConfiguration();
        }
    }
}
