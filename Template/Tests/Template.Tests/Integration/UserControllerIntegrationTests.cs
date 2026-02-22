using System.Net;
using System.Net.Http.Json;
using Template.Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Template.Tests.Integration
{
    public class UserControllerIntegrationTests : IClassFixture<WebApiTemplateFactory>
    {
        private readonly HttpClient _client;
        private readonly WebApiTemplateFactory _factory;

        public UserControllerIntegrationTests(WebApiTemplateFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var response = await _client.GetAsync("/api/user");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task GetById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = await SeedDataAsync();

            // Act
            var response = await _client.GetAsync($"/api/user/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            Assert.NotNull(user);
            Assert.Equal(userId, user.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/user/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldCreateUser()
        {
            // Arrange
            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                Name = "New test user"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/user", newUser);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();
            Assert.NotNull(createdUser);
            Assert.Equal(newUser.Name, createdUser.Name);

            // Verify it was actually created by fetching it
            var getResponse = await _client.GetAsync($"/api/user/{newUser.Id}");
            getResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenUserIsNull()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/user", (UserDto?)null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldUpdateUser()
        {
            // Arrange
            var userId = await SeedDataAsync();
            var updatedUser = new UserDto
            {
                Id = userId,
                Name = "Updated user name"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/user/{userId}", updatedUser);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/user/{userId}");
            var user = await getResponse.Content.ReadFromJsonAsync<UserDto>();
            Assert.NotNull(user);
            Assert.Equal("Updated user name", user.Name);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var user = new UserDto
            {
                Id = differentId,
                Name = "Test"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/user/{userId}", user);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var user = new UserDto
            {
                Id = nonExistentId,
                Name = "Test"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/user/{nonExistentId}", user);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldDeleteUser()
        {
            // Arrange
            var userId = await SeedDataAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/user/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/user/{userId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/user/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private async Task<Guid> SeedDataAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User"
            };
            
            context.Users.Add(user);
            await context.SaveChangesAsync();
            
            // Detach all entities to avoid tracking conflicts
            context.ChangeTracker.Clear();
            
            return user.Id;
        }
    }
}
