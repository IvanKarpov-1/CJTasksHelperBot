using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using CJTasksHelperBot.Infrastructure.Handlers;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Handlers.MessageHandlerTests;

public class HandleTextMessageAsync
{
    private readonly MessageHandler _messageHandler;

    private readonly ICommandService _commandService;
    private readonly IUserService _userService;
    private readonly IChatService _chatService;
    private readonly IUserChatService _userChatService;
    private readonly ICacheService _commandStateService;
    private readonly IStepService _stepService;
    private readonly ILocalizationService _localizationService;
    private readonly MapperlyMapper _mapper;

    public HandleTextMessageAsync()
    {
        _commandService = Substitute.For<ICommandService>();
        _userService = Substitute.For<IUserService>();
        _chatService = Substitute.For<IChatService>();
        _userChatService = Substitute.For<IUserChatService>();
        _commandStateService = Substitute.For<ICacheService>();
        _stepService = Substitute.For<IStepService>();
        _localizationService = Substitute.For<ILocalizationService>();
        _mapper = new MapperlyMapper();
        _messageHandler = Substitute.For<MessageHandler>(
            _commandService,
            _userService,
            _chatService,
            _userChatService,
            _commandStateService,
            _stepService,
            _mapper,
            _localizationService);
    }


    [Fact]
    public async Task HandleTextMessageAsync_WhenMessageNotACommandAndNoCommandExecutingStates_IgnoreMessage()
    {
        // Arrange
        const long userId = 0;
        const long chatId = 0;
        const string text = "some text";
        var message = new Message
        {
            From = new User { Id = userId },
            Chat = new Chat { Id = chatId },
            Text = text
        };
        _commandService.IsCommand(Arg.Is(text)).Returns(false);
        _commandStateService.CheckExisting(Arg.Is(userId), Arg.Is(chatId)).Returns(false);

        // Act
        await _messageHandler.HandleMessageAsync(message, default);

        // Assert
        _localizationService.DidNotReceive().SetLocalization(Arg.Any<long>());
        await _commandService.DidNotReceive().HandleTextCommandAsync(Arg.Any<UserDto>(), Arg.Any<ChatDto>(),
            Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _stepService.DidNotReceive().HandleTextCommandStepAsync(Arg.Any<UserDto>(), Arg.Any<ChatDto>(),
            Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _userChatService.DidNotReceive().FindUserChatByIdsAsync(Arg.Any<long>(), Arg.Any<long>());
        await _userService.DidNotReceive().GetUserFromTelegramModelAsync(Arg.Any<User>());
        await _chatService.DidNotReceive().GetChatFromTelegramModelAsync(Arg.Any<Chat>());
        await _userChatService.DidNotReceive().CreateUserChatAsync(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task HandleTextMessageAsync_WhenMessageIsACommand_HandleAsCommand()
    {
        // Arrange
        var faker = new AutoFaker();
        var message = faker.Generate<Message>();
        _commandService.IsCommand(Arg.Any<string>()).Returns(true);
        _commandStateService.CheckExisting(Arg.Any<long>(), Arg.Any<long>()).Returns(false);
        _userChatService.FindUserChatByIdsAsync(Arg.Any<long>(), Arg.Any<long>()).Returns(new UserChatDto());

        // Act
        await _messageHandler.HandleMessageAsync(message, default);

        // Assert
        _localizationService.Received().SetLocalization(Arg.Any<long>());
        await _userChatService.Received().FindUserChatByIdsAsync(Arg.Any<long>(), Arg.Any<long>());
        await _commandService.Received().HandleTextCommandAsync(Arg.Any<UserDto>(), Arg.Any<ChatDto>(),
            Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleTextMessageAsync_WhenMessageIsACommandExecutingState_HandleAsCommandStep()
    {
        // Arrange
        var faker = new AutoFaker();
        var message = faker.Generate<Message>();
        _commandService.IsCommand(Arg.Any<string>()).Returns(false);
        _commandStateService.CheckExisting(Arg.Any<long>(), Arg.Any<long>()).Returns(true);
        _userChatService.FindUserChatByIdsAsync(Arg.Any<long>(), Arg.Any<long>()).Returns(new UserChatDto());

        // Act
        await _messageHandler.HandleMessageAsync(message, default);

        // Assert
        _localizationService.Received().SetLocalization(Arg.Any<long>());
        await _userChatService.Received().FindUserChatByIdsAsync(Arg.Any<long>(), Arg.Any<long>());
        await _stepService.Received().HandleTextCommandStepAsync(Arg.Any<UserDto>(), Arg.Any<ChatDto>(),
            Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}