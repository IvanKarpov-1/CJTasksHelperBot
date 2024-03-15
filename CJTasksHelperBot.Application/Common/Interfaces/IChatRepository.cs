namespace CJTasksHelperBot.Application.Common.Interfaces;

public interface IChatRepository : IGenericRepository<Domain.Entities.Chat>
{
    Task<List<Domain.Entities.Chat>> GetChatsWithTasksAsync(long userId, CancellationToken cancellationToken);
}