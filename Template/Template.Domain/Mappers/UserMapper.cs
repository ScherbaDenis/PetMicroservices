using System;
using Template.Domain.DTOs;
using Template.Domain.Model;

namespace Template.Service.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User e) => e == null ? null! : new UserDto { Id = e.Id, Name = e.Name };

        public static User ToEntity(this UserDto d) => d == null ? null! : new User { Id = d.Id, Name = d.Name };
        
        public static void UpdateFromDto(this User entity, UserDto dto)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            
            entity.Name = dto.Name;
        }
    }
}
