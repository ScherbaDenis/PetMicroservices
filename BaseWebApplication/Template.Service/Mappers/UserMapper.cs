using System;
using Template.Domain.DTOs;
using Template.Domain.Model;

namespace Template.Service.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(User e) => e == null ? null! : new UserDto { Id = e.Id, Name = e.Name };

        public static User ToEntity(UserDto d) => d == null ? null! : new User { Id = d.Id, Name = d.Name };
    }
}
