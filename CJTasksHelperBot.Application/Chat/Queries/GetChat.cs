using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.Chat.Queries;

public class GetChatQuery : IRequest<Result<ChatDto>>
{
	public long ChatId { get; set; }
}

public class GetChatQueryHandler : IRequestHandler<GetChatQuery, Result<ChatDto>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public GetChatQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<ChatDto>> Handle(GetChatQuery request, CancellationToken cancellationToken)
	{
		var chat = await _unitOfWork.GetRepository<Domain.Entities.Chat>()
			.FindAsync(x => x.TelegramId == request.ChatId);

		return chat == null
			? Result<ChatDto>.Failure(new[] { $"User with id {request.ChatId} not found" })
			: Result<ChatDto>.Success(_mapper.Map(chat));
	}
}