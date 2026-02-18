using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Answer.Api.IntegrationTests.Controllers;

public class UserEndpointsTests : IClassFixture<AnswerApiFactory>
{
    private readonly HttpClient _client;

    public UserEndpointsTests(AnswerApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreatedUser()
    {
        // Arrange
        var createRequest = new { name = "John Doe" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("John Doe");
        content.Should().Contain("\"id\":");
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnEmptyList_Initially()
    {
        // Arrange
        using var client = new AnswerApiFactory().CreateClient();

        // Act
        var response = await client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"users\":");
    }

    [Fact]
    public async Task CreateAndGetUser_ShouldReturnCreatedUser()
    {
        // Arrange
        using var client = new AnswerApiFactory().CreateClient();
        var createRequest = new { name = "Jane Smith" };
        
        var createResponse = await client.PostAsJsonAsync("/api/users", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        
        // Extract ID from response (simple string parsing)
        var idStart = createContent.IndexOf("\"id\":\"") + 6;
        var idEnd = createContent.IndexOf("\"", idStart);
        var userId = createContent.Substring(idStart, idEnd - idStart);

        // Act
        var getResponse = await client.GetAsync($"/api/users/{userId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getContent = await getResponse.Content.ReadAsStringAsync();
        getContent.Should().Contain("Jane Smith");
        getContent.Should().Contain(userId);
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUserDetails()
    {
        // Arrange
        using var client = new AnswerApiFactory().CreateClient();
        var createRequest = new { name = "Original Name" };
        
        var createResponse = await client.PostAsJsonAsync("/api/users", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        
        var idStart = createContent.IndexOf("\"id\":\"") + 6;
        var idEnd = createContent.IndexOf("\"", idStart);
        var userId = createContent.Substring(idStart, idEnd - idStart);

        var updateRequest = new { name = "Updated Name" };

        // Act
        var updateResponse = await client.PutAsJsonAsync($"/api/users/{userId}", updateRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updateContent = await updateResponse.Content.ReadAsStringAsync();
        updateContent.Should().Contain("Updated Name");
    }

    [Fact]
    public async Task DeleteUser_ShouldRemoveUser()
    {
        // Arrange
        using var client = new AnswerApiFactory().CreateClient();
        var createRequest = new { name = "To Be Deleted" };
        
        var createResponse = await client.PostAsJsonAsync("/api/users", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        
        var idStart = createContent.IndexOf("\"id\":\"") + 6;
        var idEnd = createContent.IndexOf("\"", idStart);
        var userId = createContent.Substring(idStart, idEnd - idStart);

        // Act
        var deleteResponse = await client.DeleteAsync($"/api/users/{userId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var deleteContent = await deleteResponse.Content.ReadAsStringAsync();
        deleteContent.Should().Contain("\"success\":true");
    }

    [Fact]
    public async Task GetUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        using var client = new AnswerApiFactory().CreateClient();
        var invalidId = Guid.NewGuid().ToString();

        // Act
        var response = await client.GetAsync($"/api/users/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // API returns 404 for Not Found
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("not found");
    }
}
