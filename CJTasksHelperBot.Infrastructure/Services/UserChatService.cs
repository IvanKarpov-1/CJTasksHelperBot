using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.UserChat.Commands;
using CJTasksHelperBot.Application.UserChat.Queries;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using MediatR;

namespace CJTasksHelperBot.Infrastructure.Services;

public class UserChatService : IUserChatService
{
	private readonly IMediator _mediator;

	public UserChatService(IMediator mediator)
	{
		_mediator = mediator;
	}


	public async Task<UserChatDto?> FindUserChatByIdsAsync(long userId, long chatId)
	{
		var result = await _mediator.Send(new GetUserChatQuery { UserId = userId, ChatId = chatId });
		return result.Value;
	}

	public async Task CreateUserChatAsync(long userId, long chatId)
	{
		var userChatDto = await FindUserChatByIdsAsync(userId, chatId);
		if (userChatDto != null) return;

		userChatDto = new UserChatDto
		{
			UserId = userId,
			ChatId = chatId
		};

		await _mediator.Send(new CreateUserChatCommand { UserChatDto = userChatDto });
	}
}