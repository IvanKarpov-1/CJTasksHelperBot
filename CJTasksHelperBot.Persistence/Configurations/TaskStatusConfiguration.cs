using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskStatus1 = CJTasksHelperBot.Domain.Enums.TaskStatus1;

namespace CJTasksHelperBot.Persistence.Configurations;

public class TaskStatusConfiguration : IEntityTypeConfiguration<TaskStatus1>
{
	public void Configure(EntityTypeBuilder<TaskStatus1> builder)
	{
		builder
			.Property(x => x.Id)
			.ValueGeneratedNever();
	}
}