namespace CJTasksHelperBot.Persistence.DataSeeding.Stages;

public interface ICanSeedData
{
    Task SeedAsync();
}