using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.Chat.Commands;

public class CreateChatCommand : IRequest<Result<Unit>>
{
	public ChatDto? ChatDto { get; set; }
}

public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Result<Unit>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public CreateChatCommandHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<Unit>> Handle(CreateChatCommand request, CancellationToken cancellationToken)
	{
		if (request.ChatDto == null)
		{
			return Result<Unit>.Failure(new[] { "ChatDto is null" });
		}

		await _unitOfWork.GetRepository<Domain.Entities.Chat>().AddAsync(_mapper.Map(request.ChatDto));

		await _unitOfWork.CommitAsync();

		return Result<Unit>.Success(Unit.Value);
	}
}