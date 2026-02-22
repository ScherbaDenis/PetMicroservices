using Answer.Domain.Entities;
using Answer.Infrastructure.Data;
using Answer.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Answer.Infrastructure.Tests.Repositories;

public class MsSqlRepositoryTests
{
    private AnswerDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AnswerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AnswerDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity_AndReturnIt()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var repository = new MsSqlRepository<User>(context);
        var user = new User { Name = "Test User" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Name.Should().Be("Test User");
        
        // Verify it was saved to the database
        var savedUser = await context.Users.FindAsync(user.Id);
        savedUser.Should().NotBeNull();
        savedUser.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var repository = new MsSqlRepository<User>(context);
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
        using var context = CreateInMemoryDbContext();
        var repository = new MsSqlRepository<User>(context);
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
        using var context = CreateInMemoryDbContext();
        var repository = new MsSqlRepository<User>(context);
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
        using var context = CreateInMemoryDbContext();
        var repository = new MsSqlRepository<User>(context);
        var user = new User { Name = "Original Name" };
        await repository.AddAsync(user);

        // Act
        user.Name = "Updated Name";
        await repository.UpdateAsync(user);
        
        // Clear the tracked entity to ensure we're getting from DB
        context.Entry(user).State = EntityState.Detached;
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var repository = new MsSqlRepository<User>(context);
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
        using var context = CreateInMemoryDbContext();
        var repository = new MsSqlRepository<User>(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await repository.DeleteAsync(nonExistentId);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
