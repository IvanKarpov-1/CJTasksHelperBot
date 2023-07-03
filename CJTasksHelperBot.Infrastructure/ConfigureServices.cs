using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
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

		return services;
	}
}