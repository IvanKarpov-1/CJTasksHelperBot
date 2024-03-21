using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CJTasksHelperBot.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		//builder
		//	.HasAlternateKey(x => x.Id);

		//builder
		//	.HasMany(x => x.Chats)
		//	.WithMany(x => x.Users)
		//	.UsingEntity<UserChat>(
		//		l => l
		//			.HasOne<Chat>()
		//			.WithMany()
		//			.HasForeignKey(x => x.UserId)
		//			.HasPrincipalKey(nameof(User.Id))
		//			.OnDelete(DeleteBehavior.NoAction),
		//		r => r
		//			.HasOne<User>()
		//			.WithMany()
		//			.HasForeignKey(x => x.ChatId)
		//			.HasPrincipalKey(nameof(Chat.Id))
		//			.OnDelete(DeleteBehavior.NoAction));

		builder
			.Property(x => x.Id)
			.ValueGeneratedNever();

		builder
			.HasMany(x => x.Chats)
			.WithMany(x => x.Users)
			.UsingEntity<UserChat>();
	}
}