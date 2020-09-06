using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mist.Auth.Application.Interfaces;
using Mist.Auth.Application.Services;
using Mist.Auth.Domain.Mediator;
using Mist.Auth.Domain.Notifications;
using Mist.Auth.Domain.Repositories;
using Mist.Auth.Infra.Data.Context;
using Mist.Auth.Infra.Data.Repositories;

namespace Auth.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // MediatR
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserAppService, UserAppService>();

            return services;
        }
    }
}