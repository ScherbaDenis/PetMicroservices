using System.Net;
using System.Net.Http.Json;
using Comment.Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Comment.DataAccess.MsSql.Repositories;
using Comment.Domain.Models;

namespace Comment.Tests.Integration
{
    public class TemplateControllerIntegrationTests : IClassFixture<WebApiCommentFactory>
    {
        private readonly HttpClient _client;
        private readonly WebApiCommentFactory _factory;

        public TemplateControllerIntegrationTests(WebApiCommentFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllTemplates()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var response = await _client.GetAsync("/api/template");

            // Assert
            response.EnsureSuccessStatusCode();
            var templates = await response.Content.ReadFromJsonAsync<List<TemplateDto>>();
            Assert.NotNull(templates);
            Assert.NotEmpty(templates);
        }

        [Fact]
        public async Task GetById_ShouldReturnTemplate_WhenTemplateExists()
        {
            // Arrange
            var templateId = await SeedDataAsync();

            // Act
            var response = await _client.GetAsync($"/api/template/{templateId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var template = await response.Content.ReadFromJsonAsync<TemplateDto>();
            Assert.NotNull(template);
            Assert.Equal(templateId, template.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenTemplateDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/template/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldCreateTemplate()
        {
            // Arrange
            var newTemplate = new TemplateDto
            {
                Id = Guid.NewGuid(),
                Title = "New test template"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/template", newTemplate);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdTemplate = await response.Content.ReadFromJsonAsync<TemplateDto>();
            Assert.NotNull(createdTemplate);
            Assert.Equal(newTemplate.Title, createdTemplate.Title);

            // Verify it was actually created by fetching it
            var getResponse = await _client.GetAsync($"/api/template/{newTemplate.Id}");
            getResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenTemplateIsNull()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/template", (TemplateDto?)null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldUpdateTemplate()
        {
            // Arrange
            var templateId = await SeedDataAsync();
            var updatedTemplate = new TemplateDto
            {
                Id = templateId,
                Title = "Updated template title"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/template/{templateId}", updatedTemplate);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/template/{templateId}");
            var template = await getResponse.Content.ReadFromJsonAsync<TemplateDto>();
            Assert.NotNull(template);
            Assert.Equal("Updated template title", template.Title);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var template = new TemplateDto
            {
                Id = differentId,
                Title = "Test"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/template/{templateId}", template);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenTemplateDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var template = new TemplateDto
            {
                Id = nonExistentId,
                Title = "Test"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/template/{nonExistentId}", template);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldDeleteTemplate()
        {
            // Arrange
            var templateId = await SeedDataAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/template/{templateId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/template/{templateId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenTemplateDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/template/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private async Task<Guid> SeedDataAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
            
            var template = new Template
            {
                Id = Guid.NewGuid(),
                Title = "Test Template"
            };
            
            context.Templates.Add(template);
            await context.SaveChangesAsync();
            
            // Detach all entities to avoid tracking conflicts
            context.ChangeTracker.Clear();
            
            return template.Id;
        }
    }
}
