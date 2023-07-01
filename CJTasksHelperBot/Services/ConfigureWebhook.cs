using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CJTasksHelperBot.Services;

public class ConfigureWebhook : IHostedService
{
	private readonly ILogger<ConfigureWebhook> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly BotConfiguration _botConfiguration;

	public ConfigureWebhook(ILogger<ConfigureWebhook> logger, IServiceProvider serviceProvider, IOptions<BotConfiguration> botConfiguration)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_botConfiguration = botConfiguration.Value;
	}


	public async Task StartAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceProvider.CreateScope();
		var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

		var webhookAddress = $"{_botConfiguration.HostAddress}{_botConfiguration.Route}";
		_logger.LogInformation("Setting webhook: {WebhookAddress}", webhookAddress);

		await botClient.SetWebhookAsync(
			url: webhookAddress,
			allowedUpdates: Array.Empty<UpdateType>(),
			secretToken: _botConfiguration.SecretToken,
			cancellationToken: cancellationToken);
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceProvider.CreateScope();
		var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

		_logger.LogInformation("Removing webhook");
		await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
	}
}