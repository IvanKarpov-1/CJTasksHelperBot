using CJTasksHelperBot.Persistence.DataSeeding.Stages;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Persistence.DataSeeding.DataContainers;

public interface IUserDataContainer
{
    ICanAddEntity WithTasks(int count = 1, DateTime? deadline = null, TaskStatus taskStatus = TaskStatus.NotStarted);
}