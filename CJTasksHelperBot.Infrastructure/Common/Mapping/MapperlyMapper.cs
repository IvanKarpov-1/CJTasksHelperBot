using CJTasksHelperBot.Application.Common.Models;
using Riok.Mapperly.Abstractions;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Mapping;

[Mapper(PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive, UseReferenceHandling = true)]
public partial class MapperlyMapper
{
	[MapProperty(nameof(UserDto.TelegramId), nameof(User.Id))]
	public partial void Map(UserDto userDto, User user);
	[MapProperty(nameof(User.Id), nameof(UserDto.TelegramId))]
	public partial void Map(User user, UserDto userDto);
	[MapProperty(nameof(UserDto.TelegramId), nameof(User.Id))]
	public partial User Map(UserDto userDto);
	[MapProperty(nameof(User.Id), nameof(UserDto.TelegramId))]
	public partial UserDto Map(User user);


	[MapProperty(nameof(ChatDto.TelegramId), nameof(Chat.Id))]
	public partial void Map(ChatDto chatDto, Chat chat);
	[MapProperty(nameof(Chat.Id), nameof(ChatDto.TelegramId))]
	public partial void Map(Chat chat, ChatDto chatDto);
	[MapProperty(nameof(ChatDto.TelegramId), nameof(Chat.Id))]
	public partial User Map(ChatDto chatDto);
	[MapProperty(nameof(Chat.Id), nameof(ChatDto.TelegramId))]
	public partial ChatDto Map(Chat chat);

}