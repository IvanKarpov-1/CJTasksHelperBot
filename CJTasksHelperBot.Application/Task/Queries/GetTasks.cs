using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;
using System.Linq.Expressions;
using TaskStatusCustomEnum = CJTasksHelperBot.Domain.Enums.TaskStatusCustomEnum;

namespace CJTasksHelperBot.Application.Task.Queries;

public class GetTasksQuery : IRequest<Result<List<GetTaskDto>>>
{
	public long? UserId { get; set; }
	public long? ChatId { get; set; }
	public TaskStatusCustomEnum? Status { get; set; } 
}

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, Result<List<GetTaskDto>>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public GetTasksQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<List<GetTaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
	{
		Expression<Func<Domain.Entities.Task, bool>> predicate = x =>
			(request.UserId == null || x.UserChat!.UserId == request.UserId) &&
			(request.ChatId == null || x.UserChat!.ChatId == request.ChatId) &&
			(request.Status == null || x.Status.CompareTo(request.Status.Id) == 0);

		var tasks = await _unitOfWork.GetRepository<Domain.Entities.Task>().GetWhereAsync(predicate);

		var tasksDto = tasks.Select(_mapper.Map).ToList();

		return Result<List<GetTaskDto>>.Success(tasksDto);
	}
}