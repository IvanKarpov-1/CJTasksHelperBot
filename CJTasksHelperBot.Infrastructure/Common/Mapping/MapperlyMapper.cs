﻿using CJTasksHelperBot.Application.Common.Models;
using Riok.Mapperly.Abstractions;
using Telegram.Bot.Types;

namespace CJTasksHelperBot.Infrastructure.Common.Mapping;

[Mapper(PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive, UseReferenceHandling = true)]
public partial class MapperlyMapper
{
	[MapProperty(nameof(UserDto.Id), nameof(User.Id))]
	public partial void Map(UserDto userDto, User user);
	[MapProperty(nameof(User.Id), nameof(UserDto.Id))]
	public partial void Map(User user, UserDto userDto);
	[MapProperty(nameof(UserDto.Id), nameof(User.Id))]
	public partial User Map(UserDto userDto);
	[MapProperty(nameof(User.Id), nameof(UserDto.Id))]
	public partial UserDto Map(User user);


	[MapProperty(nameof(ChatDto.Id), nameof(Chat.Id))]
	public partial void Map(ChatDto chatDto, Chat chat);
	[MapProperty(nameof(Chat.Id), nameof(ChatDto.Id))]
	public partial void Map(Chat chat, ChatDto chatDto);
	[MapProperty(nameof(ChatDto.Id), nameof(Chat.Id))]
	public partial Chat Map(ChatDto chatDto);
	[MapProperty(nameof(Chat.Id), nameof(ChatDto.Id))]
	public partial ChatDto Map(Chat chat);

}