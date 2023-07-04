using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CJTasksHelperBot.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder
			.HasAlternateKey(x => x.TelegramId);

		builder
			.HasMany(x => x.Chats)
			.WithMany(x => x.Users)
			.UsingEntity<UserChat>(
				l => l
					.HasOne<Chat>()
					.WithMany()
					.HasForeignKey(x => x.UserId)
					.HasPrincipalKey(nameof(User.TelegramId))
					.OnDelete(DeleteBehavior.NoAction),
				r => r
					.HasOne<User>()
					.WithMany()
					.HasForeignKey(x => x.ChatId)
					.HasPrincipalKey(nameof(Chat.TelegramId))
					.OnDelete(DeleteBehavior.NoAction));
	}
}