using CJTasksHelperBot.Infrastructure.Common.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace CJTasksHelperBot.Infrastructure.Common.Extensions;

public static class ServiceCollectionExtensions
{
	public static void RegisterAsTransient<T>(this IServiceCollection serviceCollection)
	{
		var types = ReflectionHelper.GetImplementationsOfType(typeof(T));
		foreach (var type in types)
		{
			serviceCollection.AddTransient(typeof(T), type);
		}
	}
}