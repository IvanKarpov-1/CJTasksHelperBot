using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CJTasksHelperBot.Persistence.Tests.Unit;

public sealed class ApplicationDbContextFactory : IDisposable
{  
    private DbConnection? _connection;
    
    private DbContextOptions<ApplicationDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection!).Options;
    }
    
    public ApplicationDbContext CreateContext()
    {
        if (_connection != null) return new ApplicationDbContext(CreateOptions());
        
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = CreateOptions();
        using (var context = new ApplicationDbContext(options))
        {
            context.Database.EnsureCreated();
        }

        return new ApplicationDbContext(CreateOptions());
    }

    public void Dispose()
    {
        if (_connection == null) return;
        _connection.Dispose();
        _connection = null;
    }
}