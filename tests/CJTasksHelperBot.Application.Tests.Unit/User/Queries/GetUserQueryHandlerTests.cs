using System.Linq.Expressions;
using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.User.Queries;

namespace CJTasksHelperBot.Application.Tests.Unit.User.Queries;

public class GetUserQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly MapperlyMapper _mapper;

    public GetUserQueryHandlerTests()
    {
        _mapper = new MapperlyMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenUserIdIsNotProvided()
    {
        // Arrange
        var faker = new AutoFaker();
        var userId = faker.Generate<long>();
        var query = new GetUserQuery { UserId = userId };
        var handler = new GetUserQueryHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be($"User with id {userId} not found");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Should_ReturnFailureResult_WhenUserWithIdIsNotFound()
    {
        // Arrange
        var faker = new AutoFaker();
        var userId = faker.Generate<long>();
        var query = new GetUserQuery { UserId = userId };
        var handler = new GetUserQueryHandler(_unitOfWork, _mapper);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?[0].Should().Be($"User with id {userId} not found");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ShouldReturnUserDto_WhenUserWithIdIsFound()
    {
        // Arrange
        var faker = new AutoFaker();
        var userId = faker.Generate<long>();
        var user = faker.Generate<Domain.Entities.User>();
        var userDto = _mapper.Map(user);
        var query = new GetUserQuery { UserId = userId };
        _unitOfWork.UserRepository
            .FindAsync(Arg.Any<Expression<Func<CJTasksHelperBot.Domain.Entities.User, bool>>>(), false)
            .Returns(user);
        var handler = new GetUserQueryHandler(_unitOfWork, _mapper);
        
        // Act
        var result = await handler.Handle(query, default);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(userDto);
    }
}