using System;
using Template.Domain.DTOs;
using Template.Domain.Model;

namespace Template.Service.Mappers
{
    public static class QuestionMapper
    {
        public static QuestionDto ToDto(this Question q)
        {
            if (q == null) return null!;
            
            return q switch
            {
                SingleLineStringQuestion => new SingleLineStringQuestionDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description
                },
                MultiLineTextQuestion => new MultiLineTextQuestionDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description
                },
                PositiveIntegerQuestion => new PositiveIntegerQuestionDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description
                },
                CheckboxQuestion => new CheckboxQuestionDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description
                },
                BooleanQuestion => new BooleanQuestionDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description
                },
                _ => throw new ArgumentException($"Unknown question type: {q.GetType().Name}")
            };
        }

        public static Question ToEntity(this QuestionDto d)
        {
            if (d == null) return null!;
            
            return d switch
            {
                SingleLineStringQuestionDto dto => new SingleLineStringQuestion
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description
                },
                MultiLineTextQuestionDto dto => new MultiLineTextQuestion
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description
                },
                PositiveIntegerQuestionDto dto => new PositiveIntegerQuestion
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description
                },
                CheckboxQuestionDto dto => new CheckboxQuestion
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description
                },
                BooleanQuestionDto dto => new BooleanQuestion
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description
                },
                _ => throw new ArgumentException($"Unknown question DTO type: {d.GetType().Name}")
            };
        }
    }
}
