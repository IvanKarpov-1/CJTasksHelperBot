using CJTasksHelperBot;
using CJTasksHelperBot.Application;
using CJTasksHelperBot.Controllers;
using CJTasksHelperBot.Extensions;
using CJTasksHelperBot.Infrastructure;
using CJTasksHelperBot.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration, out var botConfiguration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddPersistenceServices(builder.Environment.IsDevelopment());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapBotWebhookRoute<BotController>(route: botConfiguration!.Route);

app.UseRequestLocalization();

app.MapControllers();

app.Run();
