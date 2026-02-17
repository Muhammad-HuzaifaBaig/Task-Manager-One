using Microsoft.Extensions.DependencyInjection;
using TaskManager.Business.Helpers;
using TaskManager.Business.Services.Auth;
using TaskManager.Business.Services.Task;
using TaskManager.Business.Services.Configuration;

namespace TaskManager.Business
{
    public static class BusinessLayer
    {
        public static IServiceCollection AddBusinessLayerServices(this IServiceCollection services)
        {
            // Register Helpers
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Register Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            // Add other services here as they are created
            // services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
