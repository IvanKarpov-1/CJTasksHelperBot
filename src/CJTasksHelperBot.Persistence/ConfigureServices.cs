using CJTasksHelperBot.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CJTasksHelperBot.Persistence;

public static class ConfigureServices
{
	public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
	{
		if (isDevelopment)
		{
			services.AddDbContext<ApplicationDbContext, SqliteApplicationDbContext>(options =>
			{
				options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
				options.EnableDetailedErrors();
				options.EnableSensitiveDataLogging();
			});
		}
		else
		{
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});
		}
		
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		return services;
	}
}