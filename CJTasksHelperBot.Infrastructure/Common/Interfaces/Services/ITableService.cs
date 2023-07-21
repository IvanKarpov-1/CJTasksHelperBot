using CJTasksHelperBot.Application.Common.Models;

namespace CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;

public interface ITableService
{
	string GetTable(IEnumerable<GetTaskDto> items);
}