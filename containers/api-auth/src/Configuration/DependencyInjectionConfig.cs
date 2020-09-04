using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Mist.Auth.Application.Interfaces;
using Mist.Auth.Application.Services;
using Mist.Auth.Domain.Mediator;
using Mist.Auth.Domain.Notifications;

namespace Auth.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            // MediatR
            services.AddMediatR(typeof(Startup));
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            services.AddScoped<IUserAppService, UserAppService>();

            return services;
        }
    }
}