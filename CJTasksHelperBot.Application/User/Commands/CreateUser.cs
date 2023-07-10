using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Entities;
using MediatR;

namespace CJTasksHelperBot.Application.User.Commands;

public class CreateUserCommand : IRequest<Result<Unit>>
{
	public UserDto? UserDto { get; set; }
	public long? ChatId { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Unit>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public CreateUserCommandHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<Unit>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
	{
		if (request.UserDto == null)
		{
			return Result<Unit>.Failure(new[] { "UserDto is null" });
		}

		_unitOfWork.GetRepository<Domain.Entities.User>().Add(_mapper.Map(request.UserDto));

		if (request.ChatId != null)
		{
			var userChatDto = new UserChatDto
			{
				ChatId = (long)request.ChatId,
				UserId = request.UserDto.Id,
				UserDto = request.UserDto
			};

			_unitOfWork.GetRepository<Domain.Entities.UserChat>().Add(_mapper.Map(userChatDto));
		}

		var result = await _unitOfWork.CommitAsync();

		return result > 0
			? Result<Unit>.Success(Unit.Value)
			: Result<Unit>.Failure(new[] { "Something went wrong whet trying to create User" });
	}
}