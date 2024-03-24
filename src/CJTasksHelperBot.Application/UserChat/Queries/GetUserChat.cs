using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.UserChat.Queries;

public class GetUserChatQuery : IRequest<Result<UserChatDto>>
{
	public long UserId { get; init; }
	public long ChatId { get; init; }
}

public class GetUserChatQueryHandler : IRequestHandler<GetUserChatQuery, Result<UserChatDto>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public GetUserChatQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<UserChatDto>> Handle(GetUserChatQuery request, CancellationToken cancellationToken)
	{
		var userChat = await _unitOfWork.UserChatRepository
			.FindAsync(x => x.UserId == request.UserId && 
			                x.ChatId == request.ChatId, false);

		return userChat == null
			? Result<UserChatDto>.Failure([$"UserChat with UserId {request.UserId} and ChatId {request.ChatId} not found"])
			: Result<UserChatDto>.Success(_mapper.Map(userChat));
	}
}