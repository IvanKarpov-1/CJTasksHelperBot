namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

public interface ICacheService
{
	void SetExpiration(int absoluteExpirationHours, int? slidingExpirationMinutes = null);
	void Add<T>(long userId, long chatId, T data);
	T? Get<T>(long userId, long chatId);
	void Update<T>(long userId, long chatId, T data);
	void Delete(long userId, long chatId);
	void Add<T>(string key, T data);
	T? Get<T>(string key);
	void Update<T>(string key, T data);
	void Delete(string key);
	bool CheckExisting(long userId, long chatId);
	bool CheckExisting(string key);
	string GenerateKey(long userId, long chatId);
	string GenerateKey();
}