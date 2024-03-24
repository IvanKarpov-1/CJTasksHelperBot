using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.Chat.Commands;

public class CreateChatCommand : IRequest<Result<Unit>>
{
	public ChatDto? ChatDto { get; init; }
	public long? UserId { get; init; }
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
			return Result<Unit>.Failure(["ChatDto is null"]);
		}

		_unitOfWork.ChatRepository.Add(_mapper.Map(request.ChatDto));

		if (request.UserId != null)
		{
			var userChatDto = new UserChatDto
			{
				UserId = (long)request.UserId,
				ChatId = request.ChatDto.Id,
				ChatDto = request.ChatDto
			};

			_unitOfWork.UserChatRepository.Add(_mapper.Map(userChatDto));
		}

		var result = await _unitOfWork.CommitAsync();

		return result > 0
			? Result<Unit>.Success(Unit.Value)
			: Result<Unit>.Failure(["Something went wrong whet trying to create Chat"]);
	}
}