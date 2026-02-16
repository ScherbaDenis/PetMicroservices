using Answer.Domain.Entities;
using Answer.Infrastructure.Repositories;
using FluentAssertions;

namespace Answer.Infrastructure.Tests.Repositories;

public class InMemoryRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddEntity_AndReturnIt()
    {
        // Arrange
        var repository = new InMemoryRepository<User>();
        var user = new User { Name = "Test User" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
    {
        // Arrange
        var repository = new InMemoryRepository<User>();
        var user = new User { Name = "Test User" };
        await repository.AddAsync(user);

        // Act
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var repository = new InMemoryRepository<User>();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var repository = new InMemoryRepository<User>();
        var user1 = new User { Name = "User 1" };
        var user2 = new User { Name = "User 2" };
        await repository.AddAsync(user1);
        await repository.AddAsync(user2);

        // Act
        var results = await repository.GetAllAsync();

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(u => u.Name == "User 1");
        results.Should().Contain(u => u.Name == "User 2");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var repository = new InMemoryRepository<User>();
        var user = new User { Name = "Original Name" };
        await repository.AddAsync(user);

        // Act
        user.Name = "Updated Name";
        await repository.UpdateAsync(user);
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var repository = new InMemoryRepository<User>();
        var user = new User { Name = "Test User" };
        await repository.AddAsync(user);

        // Act
        await repository.DeleteAsync(user.Id);
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenEntityDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryRepository<User>();
        var nonExistentId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await repository.DeleteAsync(nonExistentId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Repository_ShouldBeThreadSafe()
    {
        // Arrange
        var repository = new InMemoryRepository<User>();
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var userName = $"User {i}";
            tasks.Add(Task.Run(async () =>
            {
                var user = new User { Name = userName };
                await repository.AddAsync(user);
            }));
        }
        await Task.WhenAll(tasks);

        // Assert
        var allUsers = await repository.GetAllAsync();
        allUsers.Should().HaveCount(100);
    }
}
