using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.User.Queries;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using CJTasksHelperBot.Infrastructure.Services;
using MediatR;
using Task = System.Threading.Tasks.Task;

namespace CJTasksHelperBot.Infrastructure.Tests.Unit.Services.UserServiceTests;

public class FindUserByIdAsync
{
    private readonly UserService _userService;
    private readonly IMediator _mediator;

    public FindUserByIdAsync()
    {
        _mediator = Substitute.For<IMediator>();
        var mapper = new MapperlyMapper();
        _userService = new UserService(_mediator, mapper);
    }
    
    [Fact]
    public async Task FindUserByIdAsync_WhenUserIsNotFound_ReturnNull()
    {
        // Arrange
        var result = new Result<UserDto>();
        _mediator.Send(Arg.Any<GetUserQuery>()).Returns(result);
        
        // Act
        var userDto = await _userService.FindUserByIdAsync(default);

        // Assert
        userDto.Should().BeNull();
    }
    
    [Fact]
    public async Task FindUserByIdAsync_WhenUserIsFound_ReturnUserDto()
    {
        // Arrange
        var faker = new AutoFaker();
        var resultUserDto = faker.Generate<UserDto>();
        var result = Result<UserDto>.Success(resultUserDto);
        _mediator.Send(Arg.Any<GetUserQuery>()).Returns(result);
        
        // Act
        var userDto = await _userService.FindUserByIdAsync(default);

        // Assert
        userDto.Should().NotBeNull();
        userDto.Should().Be(resultUserDto);
    }
}