using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.User.Commands;
using CJTasksHelperBot.Application.User.Queries;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using CJTasksHelperBot.Infrastructure.Services;
using MediatR;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Services.UserServiceTests;

public class GetUserFromTelegramModelAsync
{
    private readonly UserService _userService;
    private readonly IMediator _mediator;
    private readonly MapperlyMapper _mapper;

    public GetUserFromTelegramModelAsync()
    {
        _mediator = Substitute.For<IMediator>();
        _mapper = new MapperlyMapper();
        _userService = new UserService(_mediator, _mapper);
    }

    [Fact]
    public async Task GetUserFromTelegramModelAsync_WhenUserByIdIsFound_ReturnUserDto()
    {
        // Arrange
        var faker = new AutoFaker();
        var tgUser = faker.Generate<User>();
        var resultUserDto = _mapper.Map(tgUser);
        var result = Result<UserDto>.Success(resultUserDto);
        _mediator.Send(Arg.Any<GetUserQuery>()).Returns(result);
        
        // Act
        var userDto = await _userService.GetUserFromTelegramModelAsync(tgUser);

        // Assert
        userDto.Should().NotBeNull();
        userDto.Should().BeEquivalentTo(resultUserDto);
        await _mediator.DidNotReceive().Send(Arg.Any<CreateUserCommand>());
    }

    [Fact]
    public async Task GetUserFromTelegramModelAsync_WhenUserByIdIsNotFound_CreateUserAndReturnUserDto()
    {
        // Arrange
        var faker = new AutoFaker();
        var tgUser = faker.Generate<User>();
        var resultUserDto = _mapper.Map(tgUser);
        var result = new Result<UserDto>();
        _mediator.Send(Arg.Any<GetUserQuery>()).Returns(result);
        
        // Act
        var userDto = await _userService.GetUserFromTelegramModelAsync(tgUser);
        
        // Assert
        userDto.Should().NotBeNull();
        userDto.Should().BeEquivalentTo(resultUserDto);
        await _mediator.Received().Send(Arg.Any<CreateUserCommand>());
    }
}