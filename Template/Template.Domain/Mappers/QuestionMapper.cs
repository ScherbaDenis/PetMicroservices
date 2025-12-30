using Template.Domain.DTOs;
using Template.Domain.Model;

namespace Template.Service.Mappers
{
    public static class QuestionMapper
    {
        public static QuestionDto ToDto(this Question q)
        {
            if (q == null) return null!;
            return new QuestionDto
            {
                Id = q.Id,
                Title = q.Title,
                Description = q.Description
            };
        }

        public static Question ToEntity(this QuestionDto d)
        {
            if (d == null) return null!;
            return new Question
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description
            };
        }
    }
}
