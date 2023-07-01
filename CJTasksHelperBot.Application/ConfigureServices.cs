using CJTasksHelperBot.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace CJTasksHelperBot.Application;

public static class ConfigureServices
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddScoped<MapperlyMapper>();

		return services;
	}
}