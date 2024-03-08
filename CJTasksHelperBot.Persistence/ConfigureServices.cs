using CJTasksHelperBot.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CJTasksHelperBot.Persistence;

public static class ConfigureServices
{
	public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
	{
		if (isDevelopment)
		{
			services.AddDbContext<ApplicationDbContext, SqliteApplicationDbContext>();
		}
		else
		{
			services.AddDbContext<ApplicationDbContext>();
		}
		
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		return services;
	}
}