using Bogus;
using CJTasksHelperBot.Domain.Entities;
using CJTasksHelperBot.Persistence.DataSeeding.Stages;
using Task = CJTasksHelperBot.Domain.Entities.Task;
using TaskStatus = CJTasksHelperBot.Domain.Enums.TaskStatus;

namespace CJTasksHelperBot.Persistence.DataSeeding.DataContainers;

internal class UserDataContainer(DataSeeder dataSeeder, User user, UserChat userChat) : IUserDataContainer
{
    private DataSeeder DataSeeder { get; } = dataSeeder;
    private User User { get; } = user;
    private UserChat UserChat { get; } = userChat;

    public ICanAddEntity WithTasks(int count = 1, DateTime? deadline = null, TaskStatus taskStatus = TaskStatus.NotStarted)
    {
        count = count < -1 ? 1 : count;

        UserChat.User = null;
        UserChat.Chat = null;
        
        var taskFaker = new Faker<Task>()
            .RuleFor(x => x.Id, Guid.NewGuid())
            .RuleFor(x => x.Title, f => f.Lorem.Slug())
            .RuleFor(x => x.Description, f => f.Lorem.Sentence())
            .RuleFor(x => x.UserChat, UserChat)
            .RuleFor(x => x.UserTaskStatuses, (_, _) => new List<UserTaskStatus>())
            .RuleFor(x => x.Deadline, deadline ?? DateTime.MinValue);

        var tasks = taskFaker.Generate(count);

        var userTaskStatusFaker = new Faker<UserTaskStatus>()
            .RuleFor(x => x.TaskId, f => tasks[f.IndexFaker].Id)
            .RuleFor(x => x.UserId, User.Id)
            .RuleFor(x => x.TaskStatus, taskStatus);

        var userTaskStatuses = userTaskStatusFaker.Generate(count);
        
        DataSeeder.Users.AddIfNotExists(User);
        DataSeeder.UserChats.AddIfNotExists(UserChat);
        DataSeeder.Tasks.AddIfNotExistsRange(tasks);
        DataSeeder.UserTaskStatuses.AddIfNotExistsRange(userTaskStatuses);
        
        return DataSeeder;
    }
}