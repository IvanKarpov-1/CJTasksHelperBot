using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.Chat.Queries;

public class GetChatsWithTasksQuery : IRequest<Result<List<ChatDto>>>
{
	public long UserId { get; init; }
}

public class GetChatsWithTasksQueryHandler : IRequestHandler<GetChatsWithTasksQuery, Result<List<ChatDto>>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public GetChatsWithTasksQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<List<ChatDto>>> Handle(GetChatsWithTasksQuery request, CancellationToken cancellationToken)
	{
		var chats = await _unitOfWork.ChatRepository.GetChatsWithTasksAsync(request.UserId, cancellationToken);

		var chatDtos = chats.Select(_mapper.Map).ToList();
		
		return chats.Capacity == 0
			? Result<List<ChatDto>>.Failure([$"Chats with tasks for User Id {request.UserId} not found"])
			: Result<List<ChatDto>>.Success(chatDtos);
	}
}