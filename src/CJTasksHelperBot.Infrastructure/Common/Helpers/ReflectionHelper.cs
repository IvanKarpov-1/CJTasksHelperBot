using System.Reflection;

namespace CJTasksHelperBot.Infrastructure.Common.Helpers;

public static class ReflectionHelper
{
	public static IEnumerable<T> GetInstance<T>()
	{
		var type = typeof(T);
		var types = GetImplementationsOfType(type).Where(x => !x.IsAbstract && !x.IsInterface);
		return types.Select(i => (T)Activator.CreateInstance(i)).ToList();
	}

	public static IEnumerable<Type> GetImplementationsOfType(Type type)
	{
		return Assembly.GetAssembly(type)
			.GetTypes()
			.Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract && !x.IsInterface)
			.ToList();
	}
}