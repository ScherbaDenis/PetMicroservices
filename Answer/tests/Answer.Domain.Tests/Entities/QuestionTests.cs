using Answer.Domain.Entities;
using FluentAssertions;

namespace Answer.Domain.Tests.Entities;

public class QuestionTests
{
    [Fact]
    public void Question_ShouldHaveGuidId_WhenCreated()
    {
        // Arrange & Act
        var question = new Question();

        // Assert
        question.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Question_ShouldInitializeTitle_WithEmptyString()
    {
        // Arrange & Act
        var question = new Question();

        // Assert
        question.Title.Should().NotBeNull();
        question.Title.Should().BeEmpty();
    }

    [Fact]
    public void Question_ShouldAllowSettingTitle()
    {
        // Arrange
        var question = new Question();
        const string expectedTitle = "What is Clean Architecture?";

        // Act
        question.Title = expectedTitle;

        // Assert
        question.Title.Should().Be(expectedTitle);
    }
}
