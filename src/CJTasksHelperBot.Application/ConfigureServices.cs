using CJTasksHelperBot.Application.Common.Behaviours;
using CJTasksHelperBot.Application.Common.Mapping;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CJTasksHelperBot.Application;

public static class ConfigureServices
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddScoped<MapperlyMapper>();

		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
			cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
		});

		return services;
	}
}