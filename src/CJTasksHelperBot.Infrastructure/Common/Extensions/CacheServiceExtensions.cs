using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

namespace CJTasksHelperBot.Infrastructure.Common.Extensions;

public static class CacheServiceExtensions
{
	public static void AddValueToDictionaryOfExistingStateObject(this ICacheService commandStateService,
		long userId, long chatId, string key, string value, CommandStep nextStep)
	{
		var stateObject = commandStateService.Get<StateObject>(userId, chatId);
		if (stateObject == null) return;
		stateObject.Values[key] = value;
		stateObject.CurrentStep = nextStep.DisplayName;
		commandStateService.Update(userId, chatId, stateObject);
	}
}