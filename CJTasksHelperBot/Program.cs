using CJTasksHelperBot;
using CJTasksHelperBot.Application;
using CJTasksHelperBot.Controllers;
using CJTasksHelperBot.Extensions;
using CJTasksHelperBot.Infrastructure;
using CJTasksHelperBot.Persistence;
using CJTasksHelperBot.Services;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
builder.Services.Configure<BotConfiguration>(botConfigurationSection);

var botConfiguration = botConfigurationSection.Get<BotConfiguration>();

builder.Services.AddHttpClient("telegram_bot_client")
	.AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
	{
		var botConfig = serviceProvider.GetConfiguration<BotConfiguration>();
		var options = new TelegramBotClientOptions(botConfig.BotToken);
		return new TelegramBotClient(options, httpClient);
	});

builder.Services.AddHostedService<ConfigureWebhook>();

builder.Services.AddScoped<UpdateHandlers>();

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapBotWebhookRoute<BotController>(route: botConfiguration.Route);

app.MapControllers();

app.Run();
