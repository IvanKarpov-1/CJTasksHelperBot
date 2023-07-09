using CJTasksHelperBot.Infrastructure.Common.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

namespace CJTasksHelperBot.Infrastructure.Common.Extensions;

public static class CommandStateServiceExtensions
{
	public static void AddValueToDictionaryOfExistingStateObject(this ICommandStateService commandStateService,
		long userId, long chatId, string key, string value, CommandStep nextStep)

	{
		var stateObject = commandStateService.GetStateObject<StateObject>(userId, chatId);
		if (stateObject == null) return;
		stateObject.Values[key] = value;
		stateObject.CurrentStep = nextStep.DisplayName;
		commandStateService.UpdateStateObject(userId, chatId, stateObject);
	}
}