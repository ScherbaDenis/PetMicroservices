using Template.Domain.DTOs;
using Template.Domain.Model;

namespace Template.Service.Mappers
{
    public static class TopicMapper
    {
        public static TopicDto ToDto(this Topic e) => e == null ? null! : new TopicDto { Id = e.Id, Name = e.Name };
        public static Topic ToEntity(this TopicDto d) => d == null ? null! : new Topic { Id = d.Id, Name = d.Name };
    }
}
