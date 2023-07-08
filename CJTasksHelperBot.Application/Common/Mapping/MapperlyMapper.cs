using CJTasksHelperBot.Application.Common.Models;
using Riok.Mapperly.Abstractions;
using Task = CJTasksHelperBot.Domain.Entities.Task;

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

	[MapProperty(nameof(Domain.Entities.UserChat.User), nameof(UserChatDto.UserDto))]
	[MapProperty(nameof(Domain.Entities.UserChat.Chat), nameof(UserChatDto.ChatDto))]
	public partial void Map(Domain.Entities.UserChat userChat, UserChatDto userChatDto);
	[MapProperty(nameof(UserChatDto.UserDto), nameof(Domain.Entities.UserChat.User))]
	[MapProperty(nameof(UserChatDto.ChatDto), nameof(Domain.Entities.UserChat.Chat))]
	public partial void Map(UserChatDto userChatDto, Domain.Entities.UserChat userChat);
	[MapProperty(nameof(Domain.Entities.UserChat.User), nameof(UserChatDto.UserDto))]
	[MapProperty(nameof(Domain.Entities.UserChat.Chat), nameof(UserChatDto.ChatDto))]
	public partial UserChatDto Map(Domain.Entities.UserChat userChat);
	[MapProperty(nameof(UserChatDto.UserDto), nameof(Domain.Entities.UserChat.User))]
	[MapProperty(nameof(UserChatDto.ChatDto), nameof(Domain.Entities.UserChat.Chat))]
	public partial Domain.Entities.UserChat Map(UserChatDto userChatDto);

	public partial void Map(Domain.Entities.Task task, TaskDto taskDto);
	public partial void Map(TaskDto taskDto, Domain.Entities.Task task);
	public partial TaskDto Map(Domain.Entities.Task task);
	public partial Domain.Entities.Task Map(TaskDto taskDto);
}