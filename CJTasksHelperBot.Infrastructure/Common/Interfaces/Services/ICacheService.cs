namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

public interface ICacheService
{
	void Add<T>(long userId, long chatId, T stateObject);
	T? Get<T>(long userId, long chatId);
	void Update<T>(long userId, long chatId, T stateObject);
	void Delete<T>(long userId, long chatId);
	bool CheckExisting(long userId, long chatId);
}