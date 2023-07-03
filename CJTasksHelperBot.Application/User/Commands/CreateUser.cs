using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.User.Commands;

public class CreateUserCommand : IRequest<Result<Unit>>
{
	public UserDto? UserDto { get; set; }
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

		var user = await _unitOfWork.GetRepository<Domain.Entities.User>()
			.FindAsync(x => x.TelegramId == request.UserDto.TelegramId);

		if (user != null)
		{
			return Result<Unit>.Success(Unit.Value);
		}

		await _unitOfWork.GetRepository<Domain.Entities.User>().AddAsync(_mapper.Map(request.UserDto));

		return Result<Unit>.Success(Unit.Value);
	}
}