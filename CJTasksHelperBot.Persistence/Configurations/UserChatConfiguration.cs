using CJTasksHelperBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CJTasksHelperBot.Persistence.Configurations;

public class UserChatConfiguration : IEntityTypeConfiguration<UserChat>
{
	public void Configure(EntityTypeBuilder<UserChat> builder)
	{
		builder
			.HasKey(x => new { x.UserId, x.ChatId });
	}
}