using Xunit;
using Moq;
using UserServiceTestProject.Services;
using UserServiceTestProject.DbContexts;
using UserServiceTestProject.DbContexts.DbModels;
using UserServiceTestProject.Requests;
using UserServiceTestProject.Responses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserServiceTestProject.UnitTests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task CreateUserAsync_ShouldReturnSuccess_WhenValidRequest()
        {
            //Arrange
            var mockSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<UserDbContext>(new DbContextOptions<UserDbContext>());
            mockContext.Setup(m => m.Users).Returns(mockSet.Object);
            
            var service = new UserService(mockContext.Object);
            var request = new UserCreateRequestDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Password123!",
                Role = "User"
            };

            //Act
            var result = await service.CreateUserAsync(request);

            //Assert
            Assert.True(result.Success);
            Assert.Empty(result.ErrorMessages);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldReturnError_WhenInvalidEmail()
        {
            var mockSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<UserDbContext>(new DbContextOptions<UserDbContext>());
            mockContext.Setup(m => m.Users).Returns(mockSet.Object);
            var service = new UserService(mockContext.Object);
            var request = new UserCreateRequestDto
            {
                Name = "Test User",
                Email = "invalid-email",
                Password = "Password123!",
                Role = "User"
            };
            var result = await service.CreateUserAsync(request);
            Assert.False(result.Success);
            Assert.Contains("Invalid email format.", result.ErrorMessages);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "GetAllUsersAsync_ShouldReturnAllUsers")
                .Options;

            using var context = new UserDbContext(options);
            context.Users.AddRange(new List<User> {
                new User { Id = 1, Name = "User1", Email = "user1@example.com", Role = "User", PasswordHash = "someHash123" },
                new User { Id = 2, Name = "User2", Email = "user2@example.com", Role = "Admin", PasswordHash = "someHash456" }
            });
            context.SaveChanges();
            var service = new UserService(context);


            // Act
            var result = await service.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Users.Count);
        }

        [Fact]
        public async Task UpdateUserRoleAsync_ShouldThrowException_WhenUserNotFound()
        {
            var mockSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<UserDbContext>(new DbContextOptions<UserDbContext>());
            mockContext.Setup(m => m.Users).Returns(mockSet.Object);
            mockContext.Setup(m => m.Users.FindAsync(It.IsAny<int>())).ReturnsAsync((User)null);
            var service = new UserService(mockContext.Object);
            await Assert.ThrowsAsync<System.Exception>(() => service.UpdateUserRoleAsync(1, "Admin"));
        }

        [Fact]
        public async Task UpdateUserRoleAsync_ShouldUpdateRole_WhenValid()
        {
            var user = new User { Id = 1, Name = "User1", Email = "user1@example.com", Role = "User" };
            var mockSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<UserDbContext>(new DbContextOptions<UserDbContext>());
            mockContext.Setup(m => m.Users).Returns(mockSet.Object);
            mockContext.Setup(m => m.Users.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            var service = new UserService(mockContext.Object);
            await service.UpdateUserRoleAsync(1, "Admin");
            Assert.Equal("Admin", user.Role);
        }
    }
}