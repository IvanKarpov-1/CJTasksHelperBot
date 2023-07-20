using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Interfaces;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Services;

public class CallbackQueryService : ICallbackQueryService
{
	private const string QueryTypeKey = "Query";

	private readonly IEnumerable<ICallbackQuery> _callbackQueries;
	private readonly MapperlyMapper _mapper;

	public CallbackQueryService(IEnumerable<ICallbackQuery> callbackQueries, MapperlyMapper mapper)
	{
		_callbackQueries = callbackQueries;
		_mapper = mapper;
	}

	public string CreateQuery<T>(UserDto user, params (string key, object value)[] args) where T : ICallbackQuery
	{
		var data = args.ToDictionary(k => k.key, v => v.value);
		data.Add(QueryTypeKey, typeof(T));
		return JsonConvert.SerializeObject(data);
	}

	public async Task HandleCallBackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
	{
		if (callbackQuery.Data == null) return;

		var data = ParseQueryData(callbackQuery.Data);

		if (data == null) return;

		var query = GetCallbackQuery(data);

		if (query == null) return;

		var userDto = _mapper.Map(callbackQuery.From);
		var chatDto = _mapper.Map(callbackQuery.Message!.Chat);

		await query.ExecuteAsync(userDto, chatDto, data, cancellationToken);
	}

	private Dictionary<string, object>? ParseQueryData(string data)
	{
		return JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
	}

	private ICallbackQuery? GetCallbackQuery(IReadOnlyDictionary<string, object> data)
	{
		return _callbackQueries.FirstOrDefault(x => x.GetType().Name == data[QueryTypeKey].ToString());
	}
}