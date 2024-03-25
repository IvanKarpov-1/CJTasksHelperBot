using Microsoft.EntityFrameworkCore;

namespace CJTasksHelperBot.Persistence.Tests.Unit;

public static class ApplicationDbContextFactory
{
    public static ApplicationDbContext GetApplicationDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);
        return context;
    }
}