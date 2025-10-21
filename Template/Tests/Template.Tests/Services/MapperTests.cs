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
            var dto = UserMapper.ToDto(entity);

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);

            var back = UserMapper.ToEntity(dto);
            Assert.Equal(dto.Id, back.Id);
            Assert.Equal(dto.Name, back.Name);
        }

        [Fact]
        public void TagMapper_Roundtrip_PreservesFields()
        {
            var entity = new Tag { Id = 7, Name = "tag-7" };
            var dto = TagMapper.ToDto(entity);

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);

            var back = TagMapper.ToEntity(dto);
            Assert.Equal(dto.Id, back.Id);
            Assert.Equal(dto.Name, back.Name);
        }

        [Fact]
        public void TopicMapper_Roundtrip_PreservesFields()
        {
            var entity = new Topic { Id = 3, Name = "topic-3" };
            var dto = TopicMapper.ToDto(entity);

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);

            var back = TopicMapper.ToEntity(dto);
            Assert.Equal(dto.Id, back.Id);
            Assert.Equal(dto.Name, back.Name);
        }

        [Fact]
        public void TamplateMapper_Roundtrip_PreservesFields()
        {
            var owner = new User { Id = Guid.NewGuid(), Name = "Owner" };
            var topic = new Topic { Id = 5, Name = "Topic 5" };
            var tags = new List<Tag> { new Tag { Id = 1, Name = "t1" }, new Tag { Id = 2, Name = "t2" } };

            var entity = new Tamplate
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Desc",
                Owner = owner,
                Topic = topic,
                Tags = tags
            };

            var dto = TamplateMapper.ToDto(entity);

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Title, dto.Title);
            Assert.Equal(entity.Description, dto.Description);
            Assert.Equal(entity.Owner.Id, dto.Owner!.Id);
            Assert.Equal(entity.Owner.Name, dto.Owner!.Name);
            Assert.Equal(entity.Topic.Id, dto.Topic!.Id);
            Assert.Equal(entity.Topic.Name, dto.Topic!.Name);
            Assert.Equal(entity.Tags!.ToList().Count, dto.Tags!.ToList().Count);

            var back = TamplateMapper.ToEntity(dto);

            Assert.Equal(dto.Id, back.Id);
            Assert.Equal(dto.Title, back.Title);
            Assert.Equal(dto.Description, back.Description);
            Assert.Equal(dto.Owner!.Id, back.Owner!.Id);
            Assert.Equal(dto.Owner!.Name, back.Owner!.Name);
            Assert.Equal(dto.Topic!.Id, back.Topic!.Id);
            Assert.Equal(dto.Topic!.Name, back.Topic!.Name);
            Assert.Equal(dto.Tags!.ToList().Count, back.Tags!.ToList().Count);
        }
    }
}
