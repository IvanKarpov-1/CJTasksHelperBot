using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CJTasksHelperBot.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder
			.Property(x => x.Id)
			.ValueGeneratedNever();

		builder
			.HasMany(x => x.Chats)
			.WithMany(x => x.Users)
			.UsingEntity<UserChat>();
	}
}