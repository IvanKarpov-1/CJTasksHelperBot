using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.Task.Queries;

public class GetTaskQuery : IRequest<Result<GetTaskDto>>
{
	public Guid Id { get; set; }
}

public class GetTaskQueryHandler : IRequestHandler<GetTaskQuery, Result<GetTaskDto>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public GetTaskQueryHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<GetTaskDto>> Handle(GetTaskQuery request, CancellationToken cancellationToken)
	{
		var task = await _unitOfWork.TaskRepository.GetByApplicationIdAsync(request.Id);

		return task == null
			? Result<GetTaskDto>.Failure(new[] { $"Task with Id {request.Id} not found" })
			: Result<GetTaskDto>.Success(_mapper.Map(task));
	}
}