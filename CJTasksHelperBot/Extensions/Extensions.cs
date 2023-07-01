using Microsoft.Extensions.Options;

namespace CJTasksHelperBot.Extensions;

public static class Extensions
{
    public static T GetConfiguration<T>(this IServiceProvider serviceProvider) where T : class
    {
        var options = serviceProvider.GetService<IOptions<T>>();

        return options == null ? throw new ArgumentNullException(nameof(T)) : options.Value;
    }

    public static ControllerActionEndpointConventionBuilder MapBotWebhookRoute<T>(this IEndpointRouteBuilder endpoints,
        string route)
    {
        var controllerName = typeof(T).Name.Replace("Controller", "", StringComparison.Ordinal);
        var actionName = typeof(T).GetMethods()[0].Name;

        return endpoints.MapControllerRoute(
            name: "bot_webhook",
            pattern: route,
            defaults: new
            {
                controller = controllerName,
                action = actionName
            });
    }
}