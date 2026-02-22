using Microsoft.AspNetCore.Mvc;
using Moq;
using Template.Domain.DTOs;
using Template.Service.Services;
using WebApiTemplate.Controllers;

namespace Template.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockService = new Mock<IUserService>();
            _controller = new UserController(_mockService.Object);
        }

        [Fact]
        public async void GetAll_ShouldReturnOkWithUsers()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { Id = Guid.NewGuid(), Name = "User 1" },
                new UserDto { Id = Guid.NewGuid(), Name = "User 2" }
            };
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Equal(2, returned.Count());
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserDto { Id = userId, Name = "Test user" };
            _mockService.Setup(s => s.FindAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userId, returnedUser.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockService.Setup(s => s.FindAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenUserIsValid()
        {
            // Arrange
            var user = new UserDto { Id = Guid.NewGuid(), Name = "New user" };
            _mockService.Setup(s => s.CreateAsync(user, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);

            // Act
            var result = await _controller.Create(user);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(UserController.GetById), createdResult.ActionName);
            Assert.Equal(user.Id, ((UserDto)createdResult.Value!).Id);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenUserIsNull()
        {
            // Act
            var result = await _controller.Create(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenUserIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserDto { Id = userId, Name = "Updated user" };
            _mockService.Setup(s => s.FindAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);
            _mockService.Setup(s => s.UpdateAsync(user, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(userId, user);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenUserIsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _controller.Update(userId, null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var user = new UserDto { Id = differentId, Name = "Updated user" };

            // Act
            var result = await _controller.Update(userId, user);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserDto { Id = userId, Name = "Updated user" };
            _mockService.Setup(s => s.FindAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.Update(userId, user);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserDto { Id = userId, Name = "Test user" };
            _mockService.Setup(s => s.FindAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(user);
            _mockService.Setup(s => s.DeleteAsync(user, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockService.Setup(s => s.FindAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
