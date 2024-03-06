using CJTasksHelperBot.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CJTasksHelperBot.Persistence;

public static class ConfigureServices
{
	public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
	{
		services.AddDbContext<ApplicationDbContext>(options =>
		{
			if (isDevelopment)
			{
				options.UseInMemoryDatabase(configuration.GetConnectionString("DefaultConnection") ?? "DevDB");
			}
			else
			{
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			}

			options.EnableDetailedErrors();
			options.EnableSensitiveDataLogging();
		});

		services.AddScoped<IUnitOfWork, UnitOfWork>();

		return services;
	}
}