using CJTasksHelperBot.Application.Chat.Queries;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using CJTasksHelperBot.Infrastructure.Services;
using MediatR;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Services.ChatServiceTests;

public class FindChatByIdAsync
{
    private readonly ChatService _chatService;
    private readonly IMediator _mediator;

    public FindChatByIdAsync()
    {
        _mediator = Substitute.For<IMediator>();
        var mapper = new MapperlyMapper();
        _chatService = new ChatService(_mediator, mapper);
    }

    [Fact]
    public async Task FindChatByIdAsync_WhenChatIsNotFound_ReturnNull()
    {
        // Arrange
        var result = new Result<ChatDto>();
        _mediator.Send(Arg.Any<GetChatQuery>()).Returns(result);
        
        // Act
        var chatDto = await _chatService.FindChatByIdAsync(default);

        // Assert
        chatDto.Should().BeNull();
    }

    [Fact]
    public async Task FindChatByIdAsync_WhenChatIsFound_ReturnChatDto()
    {
        // Arrange
        var faker = new AutoFaker();
        var resultChatDto = faker.Generate<ChatDto>();
        var result = Result<ChatDto>.Success(resultChatDto);
        _mediator.Send(Arg.Any<GetChatQuery>()).Returns(result);
        
        // Act
        var userDto = await _chatService.FindChatByIdAsync(default);

        // Assert
        userDto.Should().NotBeNull();
        userDto.Should().Be(resultChatDto);
    }
}