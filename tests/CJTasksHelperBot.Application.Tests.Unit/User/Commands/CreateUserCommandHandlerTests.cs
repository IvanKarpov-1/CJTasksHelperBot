using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.User.Commands;

namespace CJTasksHelperBot.Application.Tests.Unit.User.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public CreateUserCommandHandlerTests()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenUserDtoIsNull()
    {
        // Arrange
        var command = new CreateUserCommand();
        var handler = new CreateUserCommandHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("UserDto is null");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_AddUserToRepository_IfUserDtoIsNotNull()
    {
        // Arrange
        var faker = new AutoFaker();
        var userDto = faker.Generate<UserDto>();
        var command = new CreateUserCommand { UserDto = userDto };
        _unitOfWork.CommitAsync().Returns(1);
        var handler = new CreateUserCommandHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWork.UserRepository.Received().Add(Arg.Any<Domain.Entities.User>());
        await _unitOfWork.Received().CommitAsync();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_AddUserChatToRepository_IfChatIdIsNotNull()
    {
        // Arrange
        var faker = new AutoFaker();
        var userDto = faker.Generate<UserDto>();
        var chatId = faker.Generate<long>();
        var command = new CreateUserCommand { UserDto = userDto, ChatId = chatId };
        _unitOfWork.CommitAsync().Returns(1);
        var handler = new CreateUserCommandHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWork.UserChatRepository.Received().Add(Arg.Any<Domain.Entities.UserChat>());
        await _unitOfWork.Received().CommitAsync();
    }
}