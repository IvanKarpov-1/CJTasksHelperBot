﻿using CJTasksHelperBot.Infrastructure.Common.Extensions;
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
		services.AddTransient<ICacheService, CacheService>();
		services.AddTransient<IStepService, StepService>();
		services.AddTransient<ICommandHelpService, CommandHelpService>();
		services.AddTransient<ITableService, TableService>();
		services.AddTransient<ICallbackQueryService, CallbackQueryService>();

		services.AddTransient<IUpdateHandler, UpdateHandler>();
		services.AddTransient<IMessageHandler, MessageHandler>();
		services.AddTransient<ICallbackQueryHandler, CallbackQueryHandler>();

		services.RegisterAsTransient<ICommand>();
		services.RegisterAsTransient<IStep>();
		services.RegisterAsTransient<ICallbackQuery>();

		services.AddScoped<MapperlyMapper>();

		services.AddMemoryCache();

		return services;
	}
}