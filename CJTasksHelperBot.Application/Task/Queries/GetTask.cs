using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.Task.Queries;

public class GetTaskQuery : IRequest<Result<TaskDto>>
{
	public Guid Id { get; set; }
}

public class GetTaskQueryHandler : IRequestHandler<GetTaskQuery, Result<TaskDto>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public GetTaskQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<TaskDto>> Handle(GetTaskQuery request, CancellationToken cancellationToken)
	{
		var task = await _unitOfWork.GetRepository<Domain.Entities.Task>().GetByApplicationIdAsync(request.Id);

		return task == null
			? Result<TaskDto>.Failure(new[] { $"Task with Id {request.Id} not found" })
			: Result<TaskDto>.Success(_mapper.Map(task));
	}
}