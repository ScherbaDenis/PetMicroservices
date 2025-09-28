﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly TamplateDbContext _context;
        private readonly Mock<ILogger<UserRepository>> _mockLogger;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TamplateDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new TamplateDbContext(options);
            _mockLogger = new Mock<ILogger<UserRepository>>();
            _repository = new UserRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            var user = new User { Id = Guid.NewGuid(), Name = "Test User" };

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            var saved = _context.Users.FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Test User", saved!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            var user = new User { Id = Guid.NewGuid(), Name = "ToDelete" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(user);
            await _repository.SaveChangesAsync();

            Assert.Empty(_context.Users);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnUser_WhenExists()
        {
            var guid = Guid.NewGuid();
            var user = new User { Id = guid, Name = "FindMe" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _repository.FindAsync(guid);

            Assert.NotNull(result);
            Assert.Equal("FindMe", result!.Name);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.FindAsync(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public void GetAllAsync_ShouldReturnAllUsers()
        {
            _context.Users.AddRange(
                new User { Id = Guid.NewGuid(), Name = "T1" },
                new User { Id = Guid.NewGuid(), Name = "T2" }
            );
            _context.SaveChanges();

            var result = _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyUser()
        {
            var guid = Guid.NewGuid();
            var user = new User { Id = guid, Name = "OldName" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            user.Name = "NewName";
            await _repository.UpdateAsync(user);
            await _repository.SaveChangesAsync();

            var updated = _context.Users.First(t => t.Id == guid);
            Assert.Equal("NewName", updated.Name);
        }

        [Fact]
        public void Find_WithPredicate_ShouldReturnMatchingUsers()
        {
            _context.Users.AddRange(
                new User { Id = Guid.NewGuid(), Name = "Match" },
                new User { Id = Guid.NewGuid(), Name = "Skip" }
            );
            _context.SaveChanges();

            var result = _repository.Find(t => t.Name == "Match");

            Assert.Single(result);
            Assert.Equal("Match", result.First().Name);
        }
    }
}