using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Helpers;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Handlers;
using CJTasksHelperBot.Infrastructure.Identity;
using CJTasksHelperBot.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CJTasksHelperBot.Infrastructure;

public static class ConfigureServices
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
	{
		services.AddTransient<IUserService, UserService>();
		services.AddTransient<IUpdateHandler, UpdateHandler>();
		services.AddTransient<IMessageHandler, MessageHandler>();

		services.AddTransient<ICommandService, CommandService>();

		services.RegisterAsTransient<ICommand>();

		return services;
	}

	public static void RegisterAsTransient<T>(this IServiceCollection serviceCollection)
	{
		var types = ReflectionHelper.GetImplementationsOfType(typeof(T));
		foreach (var type in types)
		{
			serviceCollection.AddTransient(typeof(T), type);
		}
	}
}