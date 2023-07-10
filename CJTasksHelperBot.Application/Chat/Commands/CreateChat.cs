using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Entities;
using MediatR;

namespace CJTasksHelperBot.Application.Chat.Commands;

public class CreateChatCommand : IRequest<Result<Unit>>
{
	public ChatDto? ChatDto { get; set; }
	public long? UserId { get; set; }
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

		_unitOfWork.GetRepository<Domain.Entities.Chat>().Add(_mapper.Map(request.ChatDto));

		if (request.UserId != null)
		{
			var userChatDto = new UserChatDto
			{
				UserId = (long)request.UserId,
				ChatId = request.ChatDto.Id,
				ChatDto = request.ChatDto
			};

			_unitOfWork.GetRepository<Domain.Entities.UserChat>().Add(_mapper.Map(userChatDto));
		}

		var result = await _unitOfWork.CommitAsync();

		return result > 0
			? Result<Unit>.Success(Unit.Value)
			: Result<Unit>.Failure(new[] { "Something went wrong whet trying to create Chat" });
	}
}