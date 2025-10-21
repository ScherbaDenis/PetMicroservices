using Template.Domain.DTOs;
using Template.Domain.Model;

namespace Template.Service.Mappers
{
    public static class TagMapper
    {
        public static TagDto ToDto(this Tag e) => e == null ? null! : new TagDto { Id = e.Id, Name = e.Name };
        public static Tag ToEntity(this TagDto d) => d == null ? null! : new Tag { Id = d.Id, Name = d.Name };
    }
}
