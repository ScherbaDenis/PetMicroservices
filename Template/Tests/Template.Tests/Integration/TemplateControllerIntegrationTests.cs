using System.Net;
using System.Net.Http.Json;
using Template.Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.Tests.Integration
{
    public class TemplateControllerIntegrationTests : IClassFixture<WebApiTemplateFactory>
    {
        private readonly HttpClient _client;
        private readonly WebApiTemplateFactory _factory;

        public TemplateControllerIntegrationTests(WebApiTemplateFactory factory)
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
                Title = "New test template",
                Description = "Test description"
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
                Title = "Updated template title",
                Description = "Updated description"
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
                Title = "Test",
                Description = "Test"
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
                Title = "Test",
                Description = "Test"
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

        [Fact]
        public async Task GetByUserId_ShouldReturnUserTemplates()
        {
            // Arrange
            var (userId, templateIds) = await SeedUserWithTemplatesAsync();

            // Act
            var response = await _client.GetAsync($"/api/template/user/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var templates = await response.Content.ReadFromJsonAsync<List<TemplateDto>>();
            Assert.NotNull(templates);
            Assert.Equal(2, templates.Count);
            Assert.All(templates, t => Assert.Contains(t.Id, templateIds));
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/template/user/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnEmptyList_WhenUserHasNoTemplates()
        {
            // Arrange
            var userId = await SeedUserAsync();

            // Act
            var response = await _client.GetAsync($"/api/template/user/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var templates = await response.Content.ReadFromJsonAsync<List<TemplateDto>>();
            Assert.NotNull(templates);
            Assert.Empty(templates);
        }

        private async Task<Guid> SeedDataAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var template = new Domain.Model.Template
            {
                Id = Guid.NewGuid(),
                Title = "Test Template",
                Description = "Test Description"
            };
            
            context.Templates.Add(template);
            await context.SaveChangesAsync();
            
            // Detach all entities to avoid tracking conflicts
            context.ChangeTracker.Clear();
            
            return template.Id;
        }

        private async Task<Guid> SeedUserAsync()
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

        private async Task<(Guid userId, List<Guid> templateIds)> SeedUserWithTemplatesAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User"
            };
            
            var template1 = new Domain.Model.Template
            {
                Id = Guid.NewGuid(),
                Title = "Template 1",
                Description = "Description 1",
                Owner = user
            };
            
            var template2 = new Domain.Model.Template
            {
                Id = Guid.NewGuid(),
                Title = "Template 2",
                Description = "Description 2",
                Owner = user
            };
            
            context.Users.Add(user);
            context.Templates.Add(template1);
            context.Templates.Add(template2);
            await context.SaveChangesAsync();
            
            // Detach all entities to avoid tracking conflicts
            context.ChangeTracker.Clear();
            
            return (user.Id, new List<Guid> { template1.Id, template2.Id });
        }
    }
}
