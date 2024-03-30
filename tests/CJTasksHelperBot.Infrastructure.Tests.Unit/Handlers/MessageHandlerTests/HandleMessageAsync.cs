using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using CJTasksHelperBot.Infrastructure.Handlers;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Handlers.MessageHandlerTests;

public class HandleMessageAsync
{
    private readonly MessageHandler _messageHandler;

    private readonly ICommandService _commandService;

    public HandleMessageAsync()
    {
        _commandService = Substitute.For<ICommandService>();
        var userService = Substitute.For<IUserService>();
        var chatService = Substitute.For<IChatService>();
        var userChatService = Substitute.For<IUserChatService>();
        var commandStateService = Substitute.For<ICacheService>();
        var stepService = Substitute.For<IStepService>();
        var localizationService = Substitute.For<ILocalizationService>();
        var mapper = new MapperlyMapper();
        _messageHandler = Substitute.For<MessageHandler>(
            _commandService,
            userService,
            chatService,
            userChatService,
            commandStateService,
            stepService,
            mapper,
            localizationService);
    }

    [Fact]
    public async Task HandleMessageAsync_WhenUnknownMessageType_ExecuteHandleUnknownMessageType()
    {
        // Arrange
        var message = new Message();

        // Act
        await _messageHandler.HandleMessageAsync(message, default);

        // Assert
        await _commandService.DidNotReceive().InitializeAsync();
    }
    
    [Fact]
    public async Task HandleMessageAsync_WhenMessageTypeIsText_ExecuteHandleTextMessageAsync()
    {
        // Arrange
        var faker = new AutoFaker();
        var message = faker.Generate<Message>();

        // Act
        await _messageHandler.HandleMessageAsync(message, default);

        // Assert
        await _commandService.Received().InitializeAsync();
    }
}