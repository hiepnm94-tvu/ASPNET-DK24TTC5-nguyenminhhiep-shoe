using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using quanlybangiay.Data;

namespace quanlybangiay.Config
{
    /// <summary>
    /// Centralized database registration following ASP.NET Core guidance.
    /// Place to change provider or connection string per environment.
    /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// Registers ApplicationDbContext using the DefaultConnection from configuration.
        /// </summary>
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // Read connection string from appsettings.json (or environment-specific files)
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Register DbContext with SQL Server provider. To use a different provider (e.g. Npgsql), change this line.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}
