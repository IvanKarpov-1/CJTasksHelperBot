using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CJTasksHelperBot.Persistence.Configurations;

public class UserTaskStatusConfiguration : IEntityTypeConfiguration<UserTaskStatus>
{
    public void Configure(EntityTypeBuilder<UserTaskStatus> builder)
    {
        builder
            .HasKey(x => new { x.UserId, x.TaskId });
    }
}