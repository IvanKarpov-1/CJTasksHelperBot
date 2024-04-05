using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.UserChat.Queries;
using CJTasksHelperBot.Infrastructure.Services;
using MediatR;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Services.UserChatServiceTests;

public class FindUserChatByIdsAsync
{
    private readonly UserChatService _userChatService;
    private readonly IMediator _mediator;

    public FindUserChatByIdsAsync()
    {
        _mediator = Substitute.For<IMediator>();
        _userChatService = new UserChatService(_mediator);
    }

    [Fact]
    public async Task FindUserChatByIdsAsync_WhenUserChatIsNotFound_ReturnNull()
    {
        // Arrange
        var result = new Result<UserChatDto>();
        _mediator.Send(Arg.Any<GetUserChatQuery>()).Returns(result);
        
        // Act
        var userChatDto = await _userChatService.FindUserChatByIdsAsync(default, default);

        // Assert
        userChatDto.Should().BeNull();
    }

    [Fact]
    public async Task FindUserChatByIdsAsync_WhenUserChatIsFound_ReturnUserChatDto()
    {
        // Arrange
        var faker = new AutoFaker();
        var resultUserChatDto = faker.Generate<UserChatDto>();
        var result = Result<UserChatDto>.Success(resultUserChatDto);
        _mediator.Send(Arg.Any<GetUserChatQuery>()).Returns(result);
        
        // Act
        var userChatDto = await _userChatService.FindUserChatByIdsAsync(default, default);

        // Assert
        userChatDto.Should().NotBeNull();
        userChatDto.Should().Be(resultUserChatDto);
    }
}