using CA1.Application.DTOs;
using CA1.Application.Interfaces;
using CA1.Application.Services;
using CA1.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CA1.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _authService = new AuthService(_userRepositoryMock.Object, _jwtTokenGeneratorMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnAuthResponse_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new RegisterRequest("testuser", "password123");
        var token = "generated_token";
        
        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync((User?)null);

        _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns(token);
            
        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(token);
        result.Username.Should().Be(request.Username);
        
        _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Username == request.Username)), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenUserExists()
    {
        // Arrange
        var request = new RegisterRequest("existinguser", "password123");
        var existingUser = new User { Id = 1, Username = "existinguser" };

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(existingUser);

        // Act
        var act = async () => await _authService.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User already exists.");
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }
    
    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenUserPasswordIsInvalid()
    {
        // Arrange
        var request = new RegisterRequest("testuser", "wrongpassword");
        var existingUser = new User 
        { 
            Id = 1, 
            Username = "testuser",
            // Note: In a real test, you'd match the hashing algorithm. 
            // For this unit test with mocks, we assume the hashing in AuthService works,
            // but since we are mocking the Repo return, we need to ensure the stored hash matches what AuthService produces OR
            // simply test the 'null' user scenario first for simplicity.
            PasswordHash = "some_other_hash" 
        };

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(existingUser);

        // Act
        var act = async () => await _authService.LoginAsync(new LoginRequest("testuser", "wrongpassword"));

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid credentials.");
    }
}
