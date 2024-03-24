using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.UserChat.Commands;

namespace CJTasksHelperBot.Application.Tests.Unit.UserChat.Commands;

public class CreateUserChatCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public CreateUserChatCommandHandlerTests()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenUserChatDtoIsNull()
    {
        // Arrange
        var command = new CreateUserChatCommand();
        var handler = new CreateUserChatCommandHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be("UserChatDto is null");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenCannotAddUserChatToRepository()
    {
        // Arrange
        var faker = new AutoFaker();
        var userChatDto = faker.Generate<UserChatDto>();
        var command = new CreateUserChatCommand { UserChatDto = userChatDto };
        _unitOfWork.CommitAsync().Returns(0);
        var handler = new CreateUserChatCommandHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _unitOfWork.UserChatRepository.Received().Add(Arg.Any<Domain.Entities.UserChat>());
        result.Errors?[0].Should().Be("Something went wrong whet trying to create UserChat");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_AddUserChatToRepository_IfUserChatDtoIsNotNull()
    {
        // Arrange
        var faker = new AutoFaker();
        var userChatDto = faker.Generate<UserChatDto>();
        var command = new CreateUserChatCommand { UserChatDto = userChatDto };
        _unitOfWork.CommitAsync().Returns(1);
        var handler = new CreateUserChatCommandHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWork.UserChatRepository.Received().Add(Arg.Any<Domain.Entities.UserChat>());
        await _unitOfWork.Received().CommitAsync();
    }
}