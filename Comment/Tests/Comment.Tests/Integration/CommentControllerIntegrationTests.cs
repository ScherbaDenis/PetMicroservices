using System.Net;
using System.Net.Http.Json;
using Comment.Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Comment.DataAccess.MsSql.Repositories;
using Comment.Domain.Models;

namespace Comment.Tests.Integration
{
    public class CommentControllerIntegrationTests : IClassFixture<WebApiCommentFactory>
    {
        private readonly HttpClient _client;
        private readonly WebApiCommentFactory _factory;

        public CommentControllerIntegrationTests(WebApiCommentFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllComments()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var response = await _client.GetAsync("/api/comment");

            // Assert
            response.EnsureSuccessStatusCode();
            var comments = await response.Content.ReadFromJsonAsync<List<CommentDto>>();
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
        }

        [Fact]
        public async Task GetById_ShouldReturnComment_WhenCommentExists()
        {
            // Arrange
            var commentId = await SeedDataAsync();

            // Act
            var response = await _client.GetAsync($"/api/comment/{commentId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var comment = await response.Content.ReadFromJsonAsync<CommentDto>();
            Assert.NotNull(comment);
            Assert.Equal(commentId, comment.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/comment/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetByTemplateId_ShouldReturnComments_WhenTemplateExists()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            await SeedTemplateWithCommentsAsync(templateId);

            // Act
            var response = await _client.GetAsync($"/api/comment/template/{templateId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var comments = await response.Content.ReadFromJsonAsync<List<CommentDto>>();
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.All(comments, c => Assert.Equal(templateId, c.TemplateDto.Id));
        }

        [Fact]
        public async Task Create_ShouldCreateComment()
        {
            // Arrange
            var newComment = new CommentDto
            {
                Id = Guid.NewGuid(),
                Text = "New test comment",
                TemplateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/comment", newComment);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdComment = await response.Content.ReadFromJsonAsync<CommentDto>();
            Assert.NotNull(createdComment);
            Assert.Equal(newComment.Text, createdComment.Text);

            // Verify it was actually created by fetching it
            var getResponse = await _client.GetAsync($"/api/comment/{newComment.Id}");
            getResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenCommentIsNull()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/comment", (CommentDto?)null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldUpdateComment()
        {
            // Arrange - Seed data first
            var commentId = await SeedDataAsync();
            
            // Get the existing comment
            var getResponse = await _client.GetAsync($"/api/comment/{commentId}");
            var existingComment = await getResponse.Content.ReadFromJsonAsync<CommentDto>();
            Assert.NotNull(existingComment);
            
            // Create updated version with same template
            var updatedComment = new CommentDto
            {
                Id = existingComment.Id,
                Text = "Updated comment text",
                TemplateDto = existingComment.TemplateDto
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/comment/{commentId}", updatedComment);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var verifyResponse = await _client.GetAsync($"/api/comment/{commentId}");
            var comment = await verifyResponse.Content.ReadFromJsonAsync<CommentDto>();
            Assert.NotNull(comment);
            Assert.Equal("Updated comment text", comment.Text);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var comment = new CommentDto
            {
                Id = differentId,
                Text = "Test",
                TemplateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test" }
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/comment/{commentId}", comment);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var comment = new CommentDto
            {
                Id = nonExistentId,
                Text = "Test",
                TemplateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test" }
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/comment/{nonExistentId}", comment);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldDeleteComment()
        {
            // Arrange
            var commentId = await SeedDataAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/comment/{commentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/comment/{commentId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/comment/{nonExistentId}");

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
            
            var comment = new Domain.Models.Comment
            {
                Id = Guid.NewGuid(),
                Text = "Test comment",
                Template = template
            };
            
            context.Templates.Add(template);
            context.Comments.Add(comment);
            await context.SaveChangesAsync();
            
            // Detach all entities to avoid tracking conflicts
            context.ChangeTracker.Clear();
            
            return comment.Id;
        }

        private async Task SeedTemplateWithCommentsAsync(Guid templateId)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
            
            var template = new Template
            {
                Id = templateId,
                Title = "Test Template"
            };
            
            var comment1 = new Domain.Models.Comment
            {
                Id = Guid.NewGuid(),
                Text = "Test comment 1",
                Template = template
            };
            
            var comment2 = new Domain.Models.Comment
            {
                Id = Guid.NewGuid(),
                Text = "Test comment 2",
                Template = template
            };
            
            context.Templates.Add(template);
            context.Comments.AddRange(comment1, comment2);
            await context.SaveChangesAsync();
            
            // Detach all entities to avoid tracking conflicts
            context.ChangeTracker.Clear();
        }
    }
}
