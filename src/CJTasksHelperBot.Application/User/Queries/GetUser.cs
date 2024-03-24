using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.User.Queries;

public class GetUserQuery : IRequest<Result<UserDto>>
{
	public long UserId { get; init; }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public GetUserQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
	{
		var user = await _unitOfWork.UserRepository.FindAsync(x => x.Id == request.UserId, false);

		return user == null
			? Result<UserDto>.Failure([$"User with id {request.UserId} not found"])
			: Result<UserDto>.Success(_mapper.Map(user));
	}
}