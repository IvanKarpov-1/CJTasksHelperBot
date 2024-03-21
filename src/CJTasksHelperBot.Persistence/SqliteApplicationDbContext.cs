using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CJTasksHelperBot.Persistence;

public class SqliteApplicationDbContext : ApplicationDbContext
{
    public SqliteApplicationDbContext(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
    }
}