using Answer.Domain.Entities;
using FluentAssertions;

namespace Answer.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void User_ShouldHaveGuidId_WhenCreated()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void User_ShouldInitializeName_WithEmptyString()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Name.Should().NotBeNull();
        user.Name.Should().BeEmpty();
    }

    [Fact]
    public void User_ShouldAllowSettingName()
    {
        // Arrange
        var user = new User();
        const string expectedName = "John Doe";

        // Act
        user.Name = expectedName;

        // Assert
        user.Name.Should().Be(expectedName);
    }

    [Fact]
    public void User_Id_ShouldBeUnique()
    {
        // Arrange & Act
        var user1 = new User();
        var user2 = new User();

        // Assert
        user1.Id.Should().NotBe(user2.Id);
    }
}
