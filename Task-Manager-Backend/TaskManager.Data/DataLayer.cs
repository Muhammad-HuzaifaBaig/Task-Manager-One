using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Data.Repositories.Auth;
using TaskManager.Data.Repositories.Task;
using TaskManager.Data.Repositories.Configuration;
using TaskManager.Model.DataLayer;

namespace TaskManager.Data
{
    public static class DataLayer
    {
        public static IServiceCollection AddDataLayerServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<TaskManagerDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register Repositories
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
            // Add other repositories here as they are created
            // services.AddScoped<IUserRepository, UserRepository>();
                
            return services;
        }
    }
}
