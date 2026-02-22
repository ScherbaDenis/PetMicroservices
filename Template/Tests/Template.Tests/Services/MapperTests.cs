using System;
using System.Collections.Generic;
using System.Linq;
using Template.Domain.DTOs;
using Template.Domain.Model;
using Template.Service.Mappers;
using Xunit;

namespace Template.Tests.Services
{
    public class MapperTests
    {
        [Fact]
        public void UserMapper_Roundtrip_PreservesFields()
        {
            var entity = new User { Id = Guid.NewGuid(), Name = "Alice" };
            var dto = entity.ToDto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);

            var back = dto.ToEntity();
            Assert.Equal(dto.Id, back.Id);
            Assert.Equal(dto.Name, back.Name);
        }

        [Fact]
        public void TagMapper_Roundtrip_PreservesFields()
        {
            var entity = new Tag { Id = 7, Name = "tag-7" };
            var dto = entity.ToDto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);

            var back = dto.ToEntity();
            Assert.Equal(dto.Id, back.Id);
            Assert.Equal(dto.Name, back.Name);
        }

        [Fact]
        public void TopicMapper_Roundtrip_PreservesFields()
        {
            var entity = new Topic { Id = 3, Name = "topic-3" };
            var dto = entity.ToDto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);

            var back = dto.ToEntity();
            Assert.Equal(dto.Id, back.Id);
            Assert.Equal(dto.Name, back.Name);
        }

        [Fact]
        public void QuestionMapper_Roundtrip_PreservesFields()
        {
            var entity = new SingleLineStringQuestion { Id = Guid.NewGuid(), Title = "Question Title", Description = "Question Description" };
            var dto = entity.ToDto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Title, dto.Title);
            Assert.Equal(entity.Description, dto.Description);

            var back = dto.ToEntity();
            Assert.Equal(dto.Id, back.Id);
            Assert.Equal(dto.Title, back.Title);
            Assert.Equal(dto.Description, back.Description);
        }

        [Fact]
        public void QuestionMapper_ShouldMapSingleLineStringQuestion()
        {
            var entity = new SingleLineStringQuestion { Id = Guid.NewGuid(), Title = "Name" };
            var dto = entity.ToDto();

            Assert.IsType<SingleLineStringQuestionDto>(dto);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Title, dto.Title);
        }

        [Fact]
        public void QuestionMapper_ShouldMapMultiLineTextQuestion()
        {
            var entity = new MultiLineTextQuestion { Id = Guid.NewGuid(), Title = "Comments" };
            var dto = entity.ToDto();

            Assert.IsType<MultiLineTextQuestionDto>(dto);
            Assert.Equal(entity.Id, dto.Id);
        }

        [Fact]
        public void QuestionMapper_ShouldMapPositiveIntegerQuestion()
        {
            var entity = new PositiveIntegerQuestion { Id = Guid.NewGuid(), Title = "Age" };
            var dto = entity.ToDto();

            Assert.IsType<PositiveIntegerQuestionDto>(dto);
            Assert.Equal(entity.Id, dto.Id);
        }

        [Fact]
        public void QuestionMapper_ShouldMapCheckboxQuestion()
        {
            var entity = new CheckboxQuestion { Id = Guid.NewGuid(), Title = "Options" };
            var dto = entity.ToDto();

            Assert.IsType<CheckboxQuestionDto>(dto);
            Assert.Equal(entity.Id, dto.Id);
        }

        [Fact]
        public void QuestionMapper_ShouldMapBooleanQuestion()
        {
            var entity = new BooleanQuestion { Id = Guid.NewGuid(), Title = "Agree" };
            var dto = entity.ToDto();

            Assert.IsType<BooleanQuestionDto>(dto);
            Assert.Equal(entity.Id, dto.Id);
        }

        [Fact]
        public void QuestionMapper_ShouldMapCheckboxQuestionWithOptions()
        {
            var options = new List<string> { "Option A", "Option B", "Option C" };
            var entity = new CheckboxQuestion 
            { 
                Id = Guid.NewGuid(), 
                Title = "Choose Items",
                Options = options
            };
            var dto = entity.ToDto();

            Assert.IsType<CheckboxQuestionDto>(dto);
            var checkboxDto = dto as CheckboxQuestionDto;
            Assert.NotNull(checkboxDto);
            Assert.Equal(entity.Id, checkboxDto.Id);
            Assert.Equal(entity.Title, checkboxDto.Title);
            Assert.NotNull(checkboxDto.Options);
            Assert.Equal(3, checkboxDto.Options.Count());
            Assert.Equal("Option A", checkboxDto.Options.First());
        }

        [Fact]
        public void QuestionMapper_RoundtripCheckboxQuestionWithOptions()
        {
            var options = new List<string> { "Red", "Green", "Blue" };
            var entity = new CheckboxQuestion 
            { 
                Id = Guid.NewGuid(), 
                Title = "Select Colors",
                Description = "Pick your favorite colors",
                Options = options
            };
            
            var dto = entity.ToDto() as CheckboxQuestionDto;
            Assert.NotNull(dto);
            
            var backToEntity = dto.ToEntity() as CheckboxQuestion;
            Assert.NotNull(backToEntity);
            Assert.Equal(entity.Id, backToEntity.Id);
            Assert.Equal(entity.Title, backToEntity.Title);
            Assert.Equal(entity.Description, backToEntity.Description);
            Assert.NotNull(backToEntity.Options);
            Assert.Equal(3, backToEntity.Options.Count());
            Assert.Contains("Red", backToEntity.Options);
            Assert.Contains("Green", backToEntity.Options);
            Assert.Contains("Blue", backToEntity.Options);
        }

        [Fact]
        public void TemplateMapper_Roundtrip_PreservesFields()
        {
            var owner = new User { Id = Guid.NewGuid(), Name = "Owner" };
            var topic = new Topic { Id = 5, Name = "Topic 5" };
            var tags = new List<Tag> { new Tag { Id = 1, Name = "t1" }, new Tag { Id = 2, Name = "t2" } };
            var questions = new List<Question> 
            { 
                new SingleLineStringQuestion { Id = Guid.NewGuid(), Title = "Question 1", Description = "Desc 1" },
                new BooleanQuestion { Id = Guid.NewGuid(), Title = "Question 2", Description = "Desc 2" }
            };

            var entity = new Domain.Model.Template
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Desc",
                Owner = owner,
                Topic = topic,
                Tags = tags,
                Questions = questions
            };

            var dto = entity.ToDto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Title, dto.Title);
            Assert.Equal(entity.Description, dto.Description);
            Assert.Equal(entity.Owner.Id, dto.Owner!.Id);
            Assert.Equal(entity.Owner.Name, dto.Owner!.Name);
            Assert.Equal(entity.Topic.Id, dto.Topic!.Id);
            Assert.Equal(entity.Topic.Name, dto.Topic!.Name);
            Assert.Equal(entity.Tags!.ToList().Count, dto.Tags!.ToList().Count);
            Assert.Equal(entity.Questions!.ToList().Count, dto.Questions!.ToList().Count);

            var back = dto.ToEntity();

            Assert.Equal(dto.Id, back.Id);
            Assert.Equal(dto.Title, back.Title);
            Assert.Equal(dto.Description, back.Description);
            Assert.Equal(dto.Owner!.Id, back.Owner!.Id);
            Assert.Equal(dto.Owner!.Name, back.Owner!.Name);
            Assert.Equal(dto.Topic!.Id, back.Topic!.Id);
            Assert.Equal(dto.Topic!.Name, back.Topic!.Name);
            Assert.Equal(dto.Tags!.ToList().Count, back.Tags!.ToList().Count);
            Assert.Equal(dto.Questions!.ToList().Count, back.Questions!.ToList().Count);
        }
    }
}
