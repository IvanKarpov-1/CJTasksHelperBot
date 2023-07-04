using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CJTasksHelperBot.Persistence.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
	public void Configure(EntityTypeBuilder<Chat> builder)
	{
		builder
			.HasAlternateKey(x => x.TelegramId);

		builder
			.HasMany(x => x.Users)
			.WithMany(x => x.Chats)
			.UsingEntity<UserChat>(
				l => l
					.HasOne<User>()
					.WithMany()
					.HasForeignKey(x => x.UserId)
					.HasPrincipalKey(nameof(Chat.TelegramId))
					.OnDelete(DeleteBehavior.NoAction),
				r => r
					.HasOne<Chat>()
					.WithMany()
					.HasForeignKey(x => x.ChatId)
					.HasPrincipalKey(nameof(User.TelegramId))
					.OnDelete(DeleteBehavior.NoAction));
	}
}