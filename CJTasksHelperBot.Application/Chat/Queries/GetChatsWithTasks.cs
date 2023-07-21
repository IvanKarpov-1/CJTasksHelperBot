using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CJTasksHelperBot.Application.Chat.Queries;

public class GetChatsWithTasksQuery : IRequest<Result<List<ChatDto>>>
{
	public long UserId { get; set; }
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
		var chats = await (from userChat in _unitOfWork.GetRepository<Domain.Entities.UserChat>().GetQueryable()
				join chat in _unitOfWork.GetRepository<Domain.Entities.Chat>().GetQueryable() on userChat.ChatId equals chat.Id
				join task in _unitOfWork.GetRepository<Domain.Entities.Task>().GetQueryable() on chat.Id equals task.UserChat!.ChatId
				where userChat.UserId == request.UserId && task.UserChat!.ChatId != request.UserId
				select new ChatDto
				{
					Id = chat.Id,
					Title = chat.Title
				})
			.GroupBy(x => x.Id)
			.Select(x => new ChatDto{Id = x.Key, Title = x.First().Title })
			.ToListAsync(cancellationToken);

		//chats = chats.DistinctBy(x => x.Id).ToList();
		
				return chats.Capacity <= 0
			? Result<List<ChatDto>>.Failure(new[] { $"Chats with tasks for User Id {request.UserId} not found" })
			: Result<List<ChatDto>>.Success(chats);
	}
}