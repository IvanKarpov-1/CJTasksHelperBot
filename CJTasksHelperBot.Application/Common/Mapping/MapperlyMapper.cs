using CJTasksHelperBot.Application.Common.Models;
using Riok.Mapperly.Abstractions;

namespace CJTasksHelperBot.Application.Common.Mapping;

[Mapper(PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive, UseReferenceHandling = true)]
public partial class MapperlyMapper
{
	public partial void Map(Domain.Entities.User user, UserDto userDto);
	public partial void Map(UserDto userDto, Domain.Entities.User user);
	public partial UserDto Map(Domain.Entities.User user);
	public partial Domain.Entities.User Map(UserDto userDto);

	public partial void Map(Domain.Entities.Chat chat, ChatDto chatDto);
	public partial void Map(ChatDto chatDto, Domain.Entities.Chat chat);
	public partial ChatDto Map(Domain.Entities.Chat chat);
	public partial Domain.Entities.Chat Map(ChatDto chatDto);

	public partial void Map(Domain.Entities.UserChat userChat, UserChatDto userChatDto);
	public partial void Map(UserChatDto userChatDto, Domain.Entities.UserChat userChat);
	public partial UserChatDto Map(Domain.Entities.UserChat userChat);
	public partial Domain.Entities.UserChat Map(UserChatDto userChatDto);
}