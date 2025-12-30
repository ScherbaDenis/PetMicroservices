
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
                TamplateDto = e.Tamplate?.ToDto()
                };

        public static Models.Comment ToEntity(this CommentDto d) => 
            d == null 
            ? null! 
            : new Models.Comment { 
                Id = d.Id, 
                Text = d.Text,
                Tamplate = d.TamplateDto?.ToEntity()
            };
    }
}
