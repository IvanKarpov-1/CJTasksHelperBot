using CJTasksHelperBot.Application.Common.Models;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

public interface IDataPresentationService
{
	string GetTabledTextRepresentation(IEnumerable<GetTaskDto> items);
	string GetTabledTextRepresentation(IEnumerable<UserTaskStatusDto> items);
	string GetPlainTextRepresentation(IEnumerable<GetTaskDto> items);
	string GetPlainTextRepresentation(IEnumerable<UserTaskStatusDto> items);
}