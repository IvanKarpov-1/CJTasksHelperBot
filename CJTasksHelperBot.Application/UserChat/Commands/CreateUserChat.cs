using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.UserChat.Commands;

public class CreateUserChatCommand : IRequest<Result<Unit>>
{
	public UserChatDto? UserChatDto { get; set; }
}

public class CreateUserChatCommandHandler : IRequestHandler<CreateUserChatCommand, Result<Unit>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public CreateUserChatCommandHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<Unit>> Handle(CreateUserChatCommand request, CancellationToken cancellationToken)
	{
		if (request.UserChatDto == null)
		{
			return Result<Unit>.Failure(new[] { "UserChatDto is null" });
		}

		if (request.UserChatDto.UserId == request.UserChatDto.ChatId) return Result<Unit>.Success(Unit.Value);

		var user = await _unitOfWork.GetRepository<Domain.Entities.User>()
			.FindAsync(x => x.Id == request.UserChatDto.UserId);
		var chat = await _unitOfWork.GetRepository<Domain.Entities.Chat>()
			.FindAsync(x => x.Id == request.UserChatDto.ChatId);

		if (user == null || chat == null) return Result<Unit>.Failure(new[] { "User or Chat not found" });

		var userChat = new Domain.Entities.UserChat
		{
			UserId = user.Id,
			User = user,
			ChatId = chat.Id,
			Chat = chat
		};

		await _unitOfWork.GetRepository<Domain.Entities.UserChat>().AddAsync(userChat);

		var result = await _unitOfWork.CommitAsync();

		return result > 0
			? Result<Unit>.Success(Unit.Value)
			: Result<Unit>.Failure(new[] { "Something went wrong whet trying to create UserChat" });
	}
}