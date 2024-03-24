using CJTasksHelperBot.Application.Chat.Commands;
using CJTasksHelperBot.Application.Chat.Queries;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using MediatR;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Services;

public class ChatService : IChatService
{
	private readonly IMediator _mediator;
	private readonly MapperlyMapper _mapper;

	public ChatService(IMediator mediator, MapperlyMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}

	public async Task<ChatDto?> FindChatByIdAsync(long id)
	{
		var result = await _mediator.Send(new GetChatQuery { ChatId = id });
		return result.Value; 
	}

	public async Task<ChatDto> GetChatFromTelegramModelAsync(Chat chat)
	{
		var chatDto = await FindChatByIdAsync(chat.Id);
		if (chatDto != null) return chatDto;

		chatDto = _mapper.Map(chat);
		await _mediator.Send(new CreateChatCommand { ChatDto = chatDto });

		return chatDto;
	}
}