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
}