using System;
using System.Linq;
using Template.Domain.DTOs;
using Template.Domain.Model;

namespace Template.Service.Mappers
{
    public static class TemplateMapper
    {
        public static TemplateDto ToDto(this Domain.Model.Template e)
        {
            if (e == null) return null!;
            return new TemplateDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Owner = e.Owner == null ? null : e.Owner.ToDto(),
                Topic = e.Topic == null ? null : e.Topic.ToDto(),
                Tags = e.Tags?.Select(t => t.ToDto()).ToList() ?? new System.Collections.Generic.List<TagDto>(),
                Questions = e.Questions?.Select(q => q.ToDto()).ToList() ?? new System.Collections.Generic.List<QuestionDto>(),
                UsersAccess = e.UsersAccess?.Select(u => u.ToDto()).ToList() ?? new System.Collections.Generic.List<UserDto>()
            };
        }

        public static Domain.Model.Template ToEntity(this TemplateDto d)
        {
            if (d == null) return null!;
            return new Domain.Model.Template
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                Owner = d.Owner == null ? null : d.Owner.ToEntity(),
                Topic = d.Topic == null ? null : d.Topic.ToEntity(),
                Tags = d.Tags?.Select(t => t.ToEntity()).ToList() ?? new System.Collections.Generic.List<Tag>(),
                Questions = d.Questions?.Select(q => q.ToEntity()).ToList() ?? new System.Collections.Generic.List<Question>(),
                UsersAccess = d.UsersAccess?.Select(u => u.ToEntity()).ToList() ?? new System.Collections.Generic.List<User>()
            };
        }

        public static void UpdateFromDto(this Domain.Model.Template entity, TemplateDto dto)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            
            entity.Title = dto.Title;
            entity.Description = dto.Description;
            // Note: Complex navigation properties (Owner, Topic, Tags, Questions, UsersAccess) 
            // are typically not updated via this method to avoid EF tracking issues
        }
    }
}
