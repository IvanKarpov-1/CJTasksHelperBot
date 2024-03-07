using CJTasksHelperBot.Extensions;
using CJTasksHelperBot.Services;
using Telegram.Bot;

namespace CJTasksHelperBot;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, out BotConfiguration? botConfiguration)
    {
        var botConfigurationSection = configuration.GetSection(BotConfiguration.Configuration);
        services.Configure<BotConfiguration>(botConfigurationSection);

        botConfiguration = botConfigurationSection.Get<BotConfiguration>();
        
        services.AddControllers().AddNewtonsoftJson();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
            {
                var botConfig = serviceProvider.GetConfiguration<BotConfiguration>();
                var options = new TelegramBotClientOptions(botConfig.BotToken);
                return new TelegramBotClient(options, httpClient);
            });

        services.AddHostedService<ConfigureWebhook>();
        return services;
    }
}