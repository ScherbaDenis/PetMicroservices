using System;
using Comment.Domain.DTOs;

namespace Comment.Domain.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToDto(this Models.Comment e) => 
            e == null 
            ? null! 
            : new CommentDto { 
                Id = e.Id,  
                Text = e.Text,
                TemplateId = e.TemplateId,
                TemplateDto = e.Template?.ToDto()
                };

        public static Models.Comment ToEntity(this CommentDto d) => 
            d == null 
            ? null! 
            : new Models.Comment { 
                Id = d.Id, 
                Text = d.Text,
                TemplateId = d.TemplateId,
                Template = d.TemplateDto?.ToEntity()
            };

        public static void UpdateFromDto(this Models.Comment entity, CommentDto dto)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            
            entity.Text = dto.Text;
            // Note: Template navigation property is typically not updated to avoid EF tracking issues
        }
    }
}
