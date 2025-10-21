using System.Linq;
using Template.Domain.DTOs;
using Template.Domain.Model;

namespace Template.Service.Mappers
{
    public static class TamplateMapper
    {
        public static TamplateDto ToDto(this Tamplate e)
        {
            if (e == null) return null!;
            return new TamplateDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Owner = e.Owner == null ? null : e.Owner.ToDto(),
                Topic = e.Topic == null ? null : e.Topic.ToDto(),
                Tags = e.Tags?.Select(t => t.ToDto()).ToList() ?? new System.Collections.Generic.List<TagDto>()
            };
        }

        public static Tamplate ToEntity(this TamplateDto d)
        {
            if (d == null) return null!;
            return new Tamplate
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                Owner = d.Owner == null ? null : d.Owner.ToEntity(),
                Topic = d.Topic == null ? null : d.Topic.ToEntity(),
                Tags = d.Tags?.Select(t => t.ToEntity()).ToList() ?? new System.Collections.Generic.List<Tag>()
            };
        }
    }
}
