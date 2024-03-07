using System.Globalization;
using CJTasksHelperBot.Extensions;
using CJTasksHelperBot.Services;
using Microsoft.AspNetCore.Localization;
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

        services.AddLocalization();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new List<CultureInfo>
            {
                new("uk"),
                new("en-US"),
            };

            options.DefaultRequestCulture = new RequestCulture("uk", "uk");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders.Clear();
        });
        
        return services;
    }
}