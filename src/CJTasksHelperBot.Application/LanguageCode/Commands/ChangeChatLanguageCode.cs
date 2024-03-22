using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.LanguageCode.Commands;

public class ChangeChatLanguageCodeCommand : IRequest<Result<Unit>>
{
    public long ChatId { get; init; }
    public Domain.Enums.LanguageCode LanguageCode { get; init; }
}

public class ChangeChatLanguageCodeCommandHandler : IRequestHandler<ChangeChatLanguageCodeCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ChangeChatLanguageCodeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(ChangeChatLanguageCodeCommand request, CancellationToken cancellationToken)
    {
        var chat = await _unitOfWork.ChatRepository.FindAsync(x => x.Id == request.ChatId);

        if (chat == null) return Result<Unit>.Failure([$"Chat with id {request.ChatId} not found"]);
        
        chat.LanguageCode = request.LanguageCode;
        
        await _unitOfWork.CommitAsync();
        
        return Result<Unit>.Success(Unit.Value);
    }
}