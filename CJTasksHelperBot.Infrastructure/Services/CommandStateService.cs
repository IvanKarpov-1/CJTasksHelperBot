using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace CJTasksHelperBot.Infrastructure.Services;

public class CommandStateService : ICommandStateService
{
	private readonly IMemoryCache _memoryCache;
	private const int AbsoluteExpirationHours = 1;
	private const int SlidingExpirationMinutes = 20;

	public CommandStateService(IMemoryCache memoryCache)
	{
		_memoryCache = memoryCache;
	}

	public void AddStateObject<T>(long userId, long chatId, T stateObject)
	{
		var key = GenerateKey(userId, chatId);

		var absoluteExpiration = DateTimeOffset.Now.AddHours(AbsoluteExpirationHours);

		var entity = new CachingEntity<T>
		{
			InitialAbsoluteExpiration = absoluteExpiration,
			InitialSlidingExpiration = TimeSpan.FromMinutes(SlidingExpirationMinutes),
			Object = stateObject
		};

		_memoryCache.Set(key, entity, new MemoryCacheEntryOptions
		{
			AbsoluteExpiration = absoluteExpiration,
			SlidingExpiration = TimeSpan.FromMinutes(SlidingExpirationMinutes)
		});
	}

	public T? GetStateObject<T>(long userId, long chatId)
	{
		var key = GenerateKey(userId, chatId);
		var entity = _memoryCache.Get<CachingEntity<T>>(key);
		return entity == null ? entity!.Object : default;
	}

	public void UpdateStateObject<T>(long userId, long chatId, T stateObject)
	{
		var key = GenerateKey(userId, chatId);

		var entity = _memoryCache.Get<CachingEntity<T>>(key);

		if (entity == null) return;

		_memoryCache.Remove(key);

		_memoryCache.Set(key, entity, new MemoryCacheEntryOptions
		{
			AbsoluteExpiration = entity.InitialAbsoluteExpiration,
			SlidingExpiration = entity.InitialSlidingExpiration
		});
	}

	public void DeleteStateObject<T>(long userId, long chatId)
	{
		var key = GenerateKey(userId, chatId);
		_memoryCache.Remove(key);
	}

	public bool CheckStateObjectExisting(long userId, long chatId)
	{
		var key = GenerateKey(userId, chatId);
		var entity = _memoryCache.Get(key);
		return entity == null;
	}

	private static string GenerateKey(long userId, long chatId)
	{
		return $"{userId}{chatId}";
	}

	private class CachingEntity<T>
	{
		public DateTimeOffset? InitialAbsoluteExpiration { get; init; }
		public TimeSpan? InitialSlidingExpiration { get; init; }
		public T? Object { get; init; }
	}
}