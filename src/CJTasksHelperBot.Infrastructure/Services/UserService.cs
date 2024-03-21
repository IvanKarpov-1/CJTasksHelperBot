using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Application.User.Commands;
using CJTasksHelperBot.Application.User.Queries;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Common.Mapping;
using MediatR;
using User = Telegram.Bot.Types.User;

namespace CJTasksHelperBot.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IMediator _mediator;
    private readonly MapperlyMapper _mapper;

    public UserService(IMediator mediator, MapperlyMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<UserDto?> FindUserByIdAsync(long id)
    {
        var result = await _mediator.Send(new GetUserQuery { UserId = id });
        return result.Value;
    }

    public async Task<UserDto> GetUserFromTelegramModelAsync(User user)
    {
        var userDto = await FindUserByIdAsync(user.Id);
        if (userDto != null) return userDto;

        userDto = _mapper.Map(user);
        await _mediator.Send(new CreateUserCommand { UserDto = userDto });
        userDto = await FindUserByIdAsync(user.Id);

        return userDto!;
    }
}