using CJTasksHelperBot.Persistence.DataSeeding.DataContainers;

namespace CJTasksHelperBot.Persistence.DataSeeding.Stages;

public interface ICanAddEntity : ICanSeedData
{
    ICanAddEntity AddUsers(long chatId, params long[] userIds);
    IUserDataContainer AddUser(long chatId = 0, long userId = 0);
    ICanAddEntity AddChats(params long[] chatIds);
}