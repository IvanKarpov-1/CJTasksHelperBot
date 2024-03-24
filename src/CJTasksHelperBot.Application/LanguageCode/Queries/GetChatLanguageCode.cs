using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Enums;
using MediatR;

namespace CJTasksHelperBot.Application.LanguageCode.Queries;

public class GetChatLanguageCodeQuery : IRequest<Result<LanguageCodeCustomEnum>>
{
    public long ChatId { get; init; }
}

public class GetChatLanguageCodeQueryHandler : IRequestHandler<GetChatLanguageCodeQuery, Result<LanguageCodeCustomEnum>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetChatLanguageCodeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LanguageCodeCustomEnum>> Handle(GetChatLanguageCodeQuery request, CancellationToken cancellationToken)
    {
        var chat = await _unitOfWork.ChatRepository.FindAsync(x => x.Id == request.ChatId, false);

        return chat == null
            ? Result<LanguageCodeCustomEnum>.Failure([$"Chat with id {request.ChatId} not found"])
            : Result<LanguageCodeCustomEnum>.Success(LanguageCodeCustomEnum.FromValue((int)chat.LanguageCode));
    }
}