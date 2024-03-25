using Microsoft.EntityFrameworkCore;

namespace CJTasksHelperBot.Persistence;

public class SqliteApplicationDbContext(DbContextOptions<SqliteApplicationDbContext> options)
    : ApplicationDbContext(options);