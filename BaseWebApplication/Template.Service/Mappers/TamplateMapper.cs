using System.Linq;
using Template.Domain.DTOs;
using Template.Domain.Model;

namespace Template.Service.Mappers
{
    public static class TamplateMapper
    {
        public static TamplateDto ToDto(Tamplate e)
        {
            if (e == null) return null!;
            return new TamplateDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Owner = e.Owner == null ? null : UserMapper.ToDto(e.Owner),
                Topic = e.Topic == null ? null : TopicMapper.ToDto(e.Topic),
                Tags = e.Tags?.Select(TagMapper.ToDto).ToList() ?? new System.Collections.Generic.List<TagDto>()
            };
        }

        public static Tamplate ToEntity(TamplateDto d)
        {
            if (d == null) return null!;
            return new Tamplate
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                Owner = d.Owner == null ? null : UserMapper.ToEntity(d.Owner),
                Topic = d.Topic == null ? null : TopicMapper.ToEntity(d.Topic),
                Tags = d.Tags?.Select(TagMapper.ToEntity).ToList() ?? new System.Collections.Generic.List<Tag>()
            };
        }
    }
}
