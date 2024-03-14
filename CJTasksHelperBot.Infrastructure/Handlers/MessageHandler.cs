using System.Globalization;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Handlers;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Handlers;

public class MessageHandler : IMessageHandler
{
	private readonly ICommandService _commandService;
	private readonly IUserService _userService;
	private readonly IChatService _chatService;
	private readonly IUserChatService _userChatService;
	private readonly ICacheService _commandStateService;
	private readonly IStepService _stepService;
	private readonly ILocalizationService _localizationService;
	private readonly MapperlyMapper _mapper;

	public MessageHandler(ICommandService commandService, IUserService userService, IChatService chatService, IUserChatService userChatService, ICacheService commandStateService, IStepService stepService, MapperlyMapper mapper, ILocalizationService localizationService)
	{
		_commandService = commandService;
		_userService = userService;
		_chatService = chatService;
		_userChatService = userChatService;
		_commandStateService = commandStateService;
		_stepService = stepService;
		_mapper = mapper;
		_localizationService = localizationService;
	}

	public async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
	{
		var execute = message switch
		{
			{ Text: { } } => HandleTextMessageAsync(message, cancellationToken),
			_ => HandleUnknownMessageType(message, cancellationToken)
		};

		await execute;
	}

	public async Task HandleTextMessageAsync(Message message, CancellationToken cancellationToken)
	{
		await _commandService.InitializeAsync();

		if (_commandService.IsCommand(message.Text!))
		{
			_localizationService.SetLocalization(message.Chat.Id);
			
			var (userDto, chatDto) = await GetUserAndChat(message.From, message.Chat);

			await _commandService.HandleTextCommandAsync(userDto, chatDto, message.Text!, cancellationToken);
		}
		else if (_commandStateService.CheckExisting(message.From.Id, message.Chat.Id))
		{
			_localizationService.SetLocalization(message.Chat.Id);
			
			var (userDto, chatDto) = await GetUserAndChat(message.From, message.Chat);

			await _stepService.HandleTextCommandStepAsync(userDto, chatDto, message.Text!, cancellationToken);
		}
	}

	public Task HandleUnknownMessageType(Message message, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private async Task<(UserDto, ChatDto)> GetUserAndChat(User user, Chat chat)
	{
		var userChat = await _userChatService.FindUserChatByIdsAsync(user.Id, chat.Id);
		if (userChat != null) return (_mapper.Map(user), _mapper.Map(chat));

		var userDto = await _userService.GetUserFromTelegramModelAsync(user);
		var chatDto = await _chatService.GetChatFromTelegramModelAsync(chat);
		await _userChatService.CreateUserChatAsync(user.Id, chat.Id, false);

		return (userDto, chatDto);
	}
}