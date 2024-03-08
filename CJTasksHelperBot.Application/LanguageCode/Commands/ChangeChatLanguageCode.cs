using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.LanguageCode.Commands;

public class ChangeChatLanguageCodeQuery : IRequest<Result<Unit>>
{
    public long ChatId { get; set; }
    public Domain.Enums.LanguageCode LanguageCode { get; set; }
}

public class ChangeChatLanguageCodeQueryHandler : IRequestHandler<ChangeChatLanguageCodeQuery, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ChangeChatLanguageCodeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(ChangeChatLanguageCodeQuery request, CancellationToken cancellationToken)
    {
        var chat = await _unitOfWork
            .GetRepository<Domain.Entities.Chat>()
            .FindAsync(x => x.Id == request.ChatId);

        if (chat == null) return Result<Unit>.Failure([$"User with id {request.ChatId} not found"]);
        
        chat.LanguageCode = request.LanguageCode;
        await _unitOfWork.CommitAsync();
        return Result<Unit>.Success(Unit.Value);
    }
}