using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace CJTasksHelperBot.Infrastructure.Services;

public class CacheService : ICacheService
{
	private readonly IMemoryCache _memoryCache;
	private const int AbsoluteExpirationHours = 1;
	private const int SlidingExpirationMinutes = 20;

	public CacheService(IMemoryCache memoryCache)
	{
		_memoryCache = memoryCache;
	}

	public void Add<T>(long userId, long chatId, T stateObject)
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

	public T? Get<T>(long userId, long chatId)
	{
		var key = GenerateKey(userId, chatId);
		var entity = _memoryCache.Get<CachingEntity<T>>(key);
		return entity != null ? entity.Object : default;
	}

	public void Update<T>(long userId, long chatId, T stateObject)
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

	public void Delete<T>(long userId, long chatId)
	{
		var key = GenerateKey(userId, chatId);
		_memoryCache.Remove(key);
	}

	public bool CheckExisting(long userId, long chatId)
	{
		var key = GenerateKey(userId, chatId);
		var entity = _memoryCache.Get(key);
		return entity != null;
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