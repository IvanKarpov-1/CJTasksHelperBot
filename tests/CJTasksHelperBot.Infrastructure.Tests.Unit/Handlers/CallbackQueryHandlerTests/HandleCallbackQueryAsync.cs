using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Handlers;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Handlers.CallbackQueryHandlerTests;

public class HandleCallbackQueryAsync
{
    private readonly CallbackQueryHandler _callbackQueryHandler;
    private readonly ICallbackQueryService _callbackQueryService;

    public HandleCallbackQueryAsync()
    {
        _callbackQueryService = Substitute.For<ICallbackQueryService>();
        _callbackQueryHandler = Substitute.For<CallbackQueryHandler>(_callbackQueryService);
    }

    [Fact]
    public async Task HandleCallbackQueryAsync_ExecutesHandleCallBackQueryAsync()
    {
        // Arrange
        var callbackQuery = new CallbackQuery();

        // Act
        await _callbackQueryHandler.HandleCallbackQueryAsync(callbackQuery, default);

        // Assert
        await _callbackQueryService.Received().HandleCallBackQueryAsync(Arg.Is(callbackQuery), Arg.Any<CancellationToken>());
    }
}