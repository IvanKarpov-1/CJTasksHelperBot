namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

public interface ICommandStateService
{
	void AddStateObject<T>(long userId, long chatId, T stateObject);
	T? GetStateObject<T>(long userId, long chatId);
	void UpdateStateObject<T>(long userId, long chatId, T stateObject);
	void DeleteStateObject<T>(long userId, long chatId);
}