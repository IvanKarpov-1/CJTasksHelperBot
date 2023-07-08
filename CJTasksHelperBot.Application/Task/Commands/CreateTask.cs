using CJTasksHelperBot.Application.Common.Interfaces;
using CJTasksHelperBot.Application.Common.Mapping;
using CJTasksHelperBot.Application.Common.Models;
using MediatR;

namespace CJTasksHelperBot.Application.Task.Commands;

public class CreateTaskCommand : IRequest<Result<Unit>>
{
	public CreateTaskDto? CreateTaskDto { get; set; }
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Unit>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly MapperlyMapper _mapper;

	public CreateTaskCommandHandler(IUnitOfWork unitOfWork, MapperlyMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	public async Task<Result<Unit>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
	{
		if (request.CreateTaskDto == null)
		{
			return Result<Unit>.Failure(new[] { "TaskDto is null" });
		}

		await _unitOfWork.GetRepository<Domain.Entities.Task>().AddAsync(_mapper.Map(request.CreateTaskDto));

		var result = await _unitOfWork.CommitAsync();

		return result > 0
			? Result<Unit>.Success(Unit.Value)
			: Result<Unit>.Failure(new[] { "Something went wrong whet trying to create Task" });
	}
}