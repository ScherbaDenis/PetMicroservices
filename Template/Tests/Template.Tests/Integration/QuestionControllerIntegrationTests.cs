using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Template.Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.Tests.Integration
{
    public class QuestionControllerIntegrationTests : IClassFixture<WebApiTemplateFactory>
    {
        private readonly HttpClient _client;
        private readonly WebApiTemplateFactory _factory;

        public QuestionControllerIntegrationTests(WebApiTemplateFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetById_SingleLineStringQuestion_ShouldIncludeQuestionTypeDiscriminator()
        {
            // Arrange
            var questionId = await SeedSingleLineStringQuestionAsync();

            // Act
            var response = await _client.GetAsync($"/api/question/{questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            
            // Read as string to check raw JSON
            var jsonString = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"questionType\"", jsonString, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("\"SingleLineString\"", jsonString);

            // Also verify it deserializes correctly
            var question = await response.Content.ReadFromJsonAsync<QuestionDto>();
            Assert.NotNull(question);
            Assert.IsType<SingleLineStringQuestionDto>(question);
        }

        [Fact]
        public async Task GetById_MultiLineTextQuestion_ShouldIncludeQuestionTypeDiscriminator()
        {
            // Arrange
            var questionId = await SeedMultiLineTextQuestionAsync();

            // Act
            var response = await _client.GetAsync($"/api/question/{questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"questionType\"", jsonString, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("\"MultiLineText\"", jsonString);

            var question = await response.Content.ReadFromJsonAsync<QuestionDto>();
            Assert.NotNull(question);
            Assert.IsType<MultiLineTextQuestionDto>(question);
        }

        [Fact]
        public async Task GetById_PositiveIntegerQuestion_ShouldIncludeQuestionTypeDiscriminator()
        {
            // Arrange
            var questionId = await SeedPositiveIntegerQuestionAsync();

            // Act
            var response = await _client.GetAsync($"/api/question/{questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"questionType\"", jsonString, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("\"PositiveInteger\"", jsonString);

            var question = await response.Content.ReadFromJsonAsync<QuestionDto>();
            Assert.NotNull(question);
            Assert.IsType<PositiveIntegerQuestionDto>(question);
        }

        [Fact]
        public async Task GetById_CheckboxQuestion_ShouldIncludeQuestionTypeDiscriminator()
        {
            // Arrange
            var questionId = await SeedCheckboxQuestionAsync();

            // Act
            var response = await _client.GetAsync($"/api/question/{questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"questionType\"", jsonString, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("\"Checkbox\"", jsonString);

            var question = await response.Content.ReadFromJsonAsync<QuestionDto>();
            Assert.NotNull(question);
            Assert.IsType<CheckboxQuestionDto>(question);
            
            var checkboxQuestion = question as CheckboxQuestionDto;
            Assert.NotNull(checkboxQuestion!.Options);
            Assert.Contains("Option A", checkboxQuestion.Options);
        }

        [Fact]
        public async Task GetById_BooleanQuestion_ShouldIncludeQuestionTypeDiscriminator()
        {
            // Arrange
            var questionId = await SeedBooleanQuestionAsync();

            // Act
            var response = await _client.GetAsync($"/api/question/{questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"questionType\"", jsonString, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("\"Boolean\"", jsonString);

            var question = await response.Content.ReadFromJsonAsync<QuestionDto>();
            Assert.NotNull(question);
            Assert.IsType<BooleanQuestionDto>(question);
        }

        [Fact]
        public async Task GetAll_ShouldIncludeQuestionTypeDiscriminatorForAllQuestions()
        {
            // Arrange
            await SeedSingleLineStringQuestionAsync();
            await SeedCheckboxQuestionAsync();

            // Act
            var response = await _client.GetAsync("/api/question");

            // Assert
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"questionType\"", jsonString, StringComparison.OrdinalIgnoreCase);

            var questions = await response.Content.ReadFromJsonAsync<List<QuestionDto>>();
            Assert.NotNull(questions);
            Assert.NotEmpty(questions);
            Assert.All(questions, q => Assert.NotNull(q));
        }

        [Fact]
        public async Task Create_CheckboxQuestion_ShouldReturnResponseWithQuestionTypeDiscriminator()
        {
            // Arrange
            var newQuestion = new CheckboxQuestionDto
            {
                Id = Guid.NewGuid(),
                Title = "New checkbox question",
                Description = "Test description",
                Options = new[] { "Red", "Green", "Blue" }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/question", newQuestion);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var jsonString = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"questionType\"", jsonString, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("\"Checkbox\"", jsonString);

            var createdQuestion = await response.Content.ReadFromJsonAsync<QuestionDto>();
            Assert.NotNull(createdQuestion);
            Assert.IsType<CheckboxQuestionDto>(createdQuestion);
        }

        [Fact]
        public async Task Update_ShouldReturnSuccessAndSubsequentGetShouldIncludeDiscriminator()
        {
            // Arrange
            var questionId = await SeedSingleLineStringQuestionAsync();
            var updatedQuestion = new SingleLineStringQuestionDto
            {
                Id = questionId,
                Title = "Updated title",
                Description = "Updated description"
            };

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/question/{questionId}", updatedQuestion);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Verify the updated question still has discriminator
            var getResponse = await _client.GetAsync($"/api/question/{questionId}");
            var jsonString = await getResponse.Content.ReadAsStringAsync();
            Assert.Contains("\"questionType\"", jsonString, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("\"SingleLineString\"", jsonString);
        }

        // Helper methods to seed different question types
        private async Task<Guid> SeedSingleLineStringQuestionAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var question = new SingleLineStringQuestion
            {
                Id = Guid.NewGuid(),
                Title = "Test Single Line Question",
                Description = "Test description"
            };
            
            context.Questions.Add(question);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            
            return question.Id;
        }

        private async Task<Guid> SeedMultiLineTextQuestionAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var question = new MultiLineTextQuestion
            {
                Id = Guid.NewGuid(),
                Title = "Test Multi Line Question",
                Description = "Test description"
            };
            
            context.Questions.Add(question);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            
            return question.Id;
        }

        private async Task<Guid> SeedPositiveIntegerQuestionAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var question = new PositiveIntegerQuestion
            {
                Id = Guid.NewGuid(),
                Title = "Test Positive Integer Question",
                Description = "Test description"
            };
            
            context.Questions.Add(question);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            
            return question.Id;
        }

        private async Task<Guid> SeedCheckboxQuestionAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var question = new CheckboxQuestion
            {
                Id = Guid.NewGuid(),
                Title = "Test Checkbox Question",
                Description = "Test description",
                Options = new[] { "Option A", "Option B", "Option C" }
            };
            
            context.Questions.Add(question);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            
            return question.Id;
        }

        private async Task<Guid> SeedBooleanQuestionAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var question = new BooleanQuestion
            {
                Id = Guid.NewGuid(),
                Title = "Test Boolean Question",
                Description = "Test description"
            };
            
            context.Questions.Add(question);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            
            return question.Id;
        }
    }
}
