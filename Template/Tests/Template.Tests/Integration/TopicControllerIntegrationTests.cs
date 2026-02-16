using System.Net;
using System.Net.Http.Json;
using Template.Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Template.Tests.Integration
{
    public class TopicControllerIntegrationTests : IClassFixture<WebApiTemplateFactory>
    {
        private readonly HttpClient _client;
        private readonly WebApiTemplateFactory _factory;

        public TopicControllerIntegrationTests(WebApiTemplateFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllTopics()
        {
            // Arrange
            await SeedDataAsync();

            // Act
            var response = await _client.GetAsync("/api/topic");

            // Assert
            response.EnsureSuccessStatusCode();
            var topics = await response.Content.ReadFromJsonAsync<List<TopicDto>>();
            Assert.NotNull(topics);
            Assert.NotEmpty(topics);
        }

        [Fact]
        public async Task GetById_ShouldReturnTopic_WhenTopicExists()
        {
            // Arrange
            var topicId = await SeedDataAsync();

            // Act
            var response = await _client.GetAsync($"/api/topic/{topicId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var topic = await response.Content.ReadFromJsonAsync<TopicDto>();
            Assert.NotNull(topic);
            Assert.Equal(topicId, topic.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenTopicDoesNotExist()
        {
            // Arrange
            var nonExistentId = 99999;

            // Act
            var response = await _client.GetAsync($"/api/topic/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldCreateTopic()
        {
            // Arrange
            var newTopic = new TopicDto
            {
                Id = 0, // Auto-generated for new entities
                Name = "New test topic"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/topic", newTopic);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdTopic = await response.Content.ReadFromJsonAsync<TopicDto>();
            Assert.NotNull(createdTopic);
            Assert.Equal(newTopic.Name, createdTopic.Name);

            // Verify it was actually created by fetching it
            var getResponse = await _client.GetAsync($"/api/topic/{createdTopic.Id}");
            getResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenTopicIsNull()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/topic", (TopicDto?)null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldUpdateTopic()
        {
            // Arrange
            var topicId = await SeedDataAsync();
            var updatedTopic = new TopicDto
            {
                Id = topicId,
                Name = "Updated topic name"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/topic/{topicId}", updatedTopic);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/topic/{topicId}");
            var topic = await getResponse.Content.ReadFromJsonAsync<TopicDto>();
            Assert.NotNull(topic);
            Assert.Equal("Updated topic name", topic.Name);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var topicId = 1;
            var differentId = 2;
            var topic = new TopicDto
            {
                Id = differentId,
                Name = "Test"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/topic/{topicId}", topic);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenTopicDoesNotExist()
        {
            // Arrange
            var nonExistentId = 99999;
            var topic = new TopicDto
            {
                Id = nonExistentId,
                Name = "Test"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/topic/{nonExistentId}", topic);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldDeleteTopic()
        {
            // Arrange
            var topicId = await SeedDataAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/topic/{topicId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/topic/{topicId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenTopicDoesNotExist()
        {
            // Arrange
            var nonExistentId = 99999;

            // Act
            var response = await _client.DeleteAsync($"/api/topic/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private async Task<int> SeedDataAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var topic = new Topic
            {
                Name = "Test Topic"
            };
            
            context.Topics.Add(topic);
            await context.SaveChangesAsync();
            
            // Detach all entities to avoid tracking conflicts
            context.ChangeTracker.Clear();
            
            return topic.Id;
        }
    }
}
