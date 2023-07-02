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
	}
}