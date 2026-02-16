using Answer.Domain.Entities;
using FluentAssertions;

namespace Answer.Domain.Tests.Entities;

public class AnswerTests
{
    [Fact]
    public void Answer_ShouldHaveGuidId_WhenCreated()
    {
        // Arrange & Act
        var answer = new Domain.Entities.Answer();

        // Assert
        answer.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Answer_ShouldInitializeAnswerValue_WithEmptyString()
    {
        // Arrange & Act
        var answer = new Domain.Entities.Answer();

        // Assert
        answer.AnswerValue.Should().NotBeNull();
        answer.AnswerValue.Should().BeEmpty();
    }

    [Fact]
    public void Answer_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var templateId = Guid.NewGuid();
        var answerType = AnswerType.SingleLineString;
        const string answerValue = "Test Answer";

        // Act
        var answer = new Domain.Entities.Answer
        {
            UserId = userId,
            QuestionId = questionId,
            TemplateId = templateId,
            AnswerType = answerType,
            AnswerValue = answerValue
        };

        // Assert
        answer.UserId.Should().Be(userId);
        answer.QuestionId.Should().Be(questionId);
        answer.TemplateId.Should().Be(templateId);
        answer.AnswerType.Should().Be(answerType);
        answer.AnswerValue.Should().Be(answerValue);
    }

    [Theory]
    [InlineData(AnswerType.SingleLineString)]
    [InlineData(AnswerType.MultiLineText)]
    [InlineData(AnswerType.PositiveInteger)]
    [InlineData(AnswerType.Checkbox)]
    [InlineData(AnswerType.Boolean)]
    public void Answer_ShouldAcceptAllAnswerTypes(AnswerType answerType)
    {
        // Arrange
        var answer = new Domain.Entities.Answer();

        // Act
        answer.AnswerType = answerType;

        // Assert
        answer.AnswerType.Should().Be(answerType);
    }
}
