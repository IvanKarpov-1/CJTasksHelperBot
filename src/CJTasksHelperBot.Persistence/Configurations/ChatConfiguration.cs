using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CJTasksHelperBot.Persistence.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
	public void Configure(EntityTypeBuilder<Chat> builder)
	{
		builder
			.Property(x => x.Id)
			.ValueGeneratedNever();

		builder
			.HasMany(x => x.Users)
			.WithMany(x => x.Chats)
			.UsingEntity<UserChat>();
	}
}