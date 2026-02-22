using System.Net;
using System.Net.Http.Json;
using Template.Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Template.Tests.Integration
{
    public class TagControllerIntegrationTests : IClassFixture<WebApiTemplateFactory>
    {
        private readonly HttpClient _client;
        private readonly WebApiTemplateFactory _factory;

        public TagControllerIntegrationTests(WebApiTemplateFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllTags()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var response = await _client.GetAsync("/api/tag");

            // Assert
            response.EnsureSuccessStatusCode();
            var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>();
            Assert.NotNull(tags);
            Assert.NotEmpty(tags);
        }

        [Fact]
        public async Task GetById_ShouldReturnTag_WhenTagExists()
        {
            // Arrange
            var tagId = await SeedDataAsync();

            // Act
            var response = await _client.GetAsync($"/api/tag/{tagId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var tag = await response.Content.ReadFromJsonAsync<TagDto>();
            Assert.NotNull(tag);
            Assert.Equal(tagId, tag.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenTagDoesNotExist()
        {
            // Arrange
            var nonExistentId = 99999;

            // Act
            var response = await _client.GetAsync($"/api/tag/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldCreateTag()
        {
            // Arrange
            var newTag = new TagDto
            {
                Id = 0, // Auto-generated for new entities
                Name = "New test tag"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tag", newTag);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdTag = await response.Content.ReadFromJsonAsync<TagDto>();
            Assert.NotNull(createdTag);
            Assert.Equal(newTag.Name, createdTag.Name);

            // Verify it was actually created by fetching it
            var getResponse = await _client.GetAsync($"/api/tag/{createdTag.Id}");
            getResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenTagIsNull()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/tag", (TagDto?)null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldUpdateTag()
        {
            // Arrange
            var tagId = await SeedDataAsync();
            var updatedTag = new TagDto
            {
                Id = tagId,
                Name = "Updated tag name"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tag/{tagId}", updatedTag);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/tag/{tagId}");
            var tag = await getResponse.Content.ReadFromJsonAsync<TagDto>();
            Assert.NotNull(tag);
            Assert.Equal("Updated tag name", tag.Name);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var tagId = 1;
            var differentId = 2;
            var tag = new TagDto
            {
                Id = differentId,
                Name = "Test"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tag/{tagId}", tag);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenTagDoesNotExist()
        {
            // Arrange
            var nonExistentId = 99999;
            var tag = new TagDto
            {
                Id = nonExistentId,
                Name = "Test"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/tag/{nonExistentId}", tag);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldDeleteTag()
        {
            // Arrange
            var tagId = await SeedDataAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/tag/{tagId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/tag/{tagId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenTagDoesNotExist()
        {
            // Arrange
            var nonExistentId = 99999;

            // Act
            var response = await _client.DeleteAsync($"/api/tag/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private async Task<int> SeedDataAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var tag = new Tag
            {
                Name = "Test Tag"
            };
            
            context.Tags.Add(tag);
            await context.SaveChangesAsync();
            
            // Detach all entities to avoid tracking conflicts
            context.ChangeTracker.Clear();
            
            return tag.Id;
        }
    }
}
