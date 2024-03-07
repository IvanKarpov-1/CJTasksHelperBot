using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Enums;
using MediatR;

namespace CJTasksHelperBot.Application.LanguageCode.Queries;

public class GetUserLanguageCodeQuery : IRequest<Result<Domain.Enums.LanguageCodeCustomEnum>>
{
    public long UserId { get; set; }
}

public class GetUserLanguageCodeQueryHandler : IRequestHandler<GetUserLanguageCodeQuery, Result<LanguageCodeCustomEnum>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserLanguageCodeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LanguageCodeCustomEnum>> Handle(GetUserLanguageCodeQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.GetRepository<Domain.Entities.User>()
            .FindAsync(x => x.Id == request.UserId, false);

        return user == null
            ? Result<LanguageCodeCustomEnum>.Failure(new[] { $"User with id {request.UserId} not found" })
            : Result<LanguageCodeCustomEnum>.Success(LanguageCodeCustomEnum.FromValue((int)user.LanguageCode));
    }
}