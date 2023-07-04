using CJTasksHelperBot.Infrastructure.Common.Extensions;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using CJTasksHelperBot.Infrastructure.Handlers;
using CJTasksHelperBot.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CJTasksHelperBot.Infrastructure;

public static class ConfigureServices
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
	{
		services.AddTransient<IUserService, UserService>();
		services.AddTransient<IChatService, ChatService>();
		services.AddTransient<IUserChatService, UserChatService>();
		services.AddTransient<ICommandService, CommandService>();

		services.AddTransient<IUpdateHandler, UpdateHandler>();
		services.AddTransient<IMessageHandler, MessageHandler>();

		services.RegisterAsTransient<ICommand>();

		services.AddScoped<MapperlyMapper>();

		return services;
	}
}