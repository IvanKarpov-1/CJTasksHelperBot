using CJTasksHelperBot.Persistence.Repositories;
using Task = System.Threading.Tasks.Task;
using DataSeeder = CJTasksHelperBot.Persistence.DataSeeding.DataSeeder;

namespace CJTasksHelperBot.Persistence.Tests.Unit.Repository.ChatRepositoryTests;

public class GetChatsWithTasksAsyncTests
{
    [Fact]
    public async Task GetChatsWithTasksAsync_WhenChatsNotFound_ReturnEmptyList()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var chatRepository = new ChatRepository(context);
        const long existingUserId = 0;
        const long neededUserId = 1;
        await DataSeeder
            .CreateSeeder(context)
            .AddUsers(0, existingUserId)
            .SeedAsync();

        // Act
        var result = await chatRepository.GetChatsWithTasksAsync(neededUserId, default);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetChatsWithTasksAsync_WhenChatsAreFound_ReturnChats()
    {
        // Arrange
        using var factory = new ApplicationDbContextFactory();
        await using var context = factory.CreateContext();
        var chatRepository = new ChatRepository(context);
        const long neededUserId = 2;
        long[] chatIds = [0, 1, 2];
        await DataSeeder
            .CreateSeeder(context)
            .AddUser(chatIds[0])
            .WithTasks()
            .AddUser(chatIds[0], neededUserId)
            .WithTasks()
            .AddUser(chatIds[1], neededUserId)
            .WithTasks()
            .AddUser(neededUserId, neededUserId)
            .WithTasks()
            .AddUser(chatIds[2], 2)
            .WithTasks()
            .SeedAsync();

        // Act
        var result = await chatRepository.GetChatsWithTasksAsync(neededUserId, default);

        // Assert
        result.Should().NotBeEmpty();
        result.Count.Should().Be(2);
        for (var i = 0; i < 2; i++)
        {
            result[i].Id.Should().Be(chatIds[i]);
        }
    }
}