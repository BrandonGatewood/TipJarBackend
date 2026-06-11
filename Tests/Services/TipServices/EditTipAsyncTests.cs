using Microsoft.Extensions.Caching.Distributed;
using Moq;
using TipJar.Application.Exceptions;
using TipJar.Application.Services;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Services;
using Xunit;

namespace TipJar.Tests.Services.TipServices;

public class EditTipAsyncTests
{
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ITipRepository> _tipRepository = new();
    private readonly Mock<IDistributedCache> _cache = new();

    private TipService CreateService()
    {
        return new TipService(
            _userService.Object,
            _userRepository.Object,
            _tipRepository.Object,
            _cache.Object
        );
    }

    [Fact]
    public async Task ValidInput_UpdatesTip()
    {
        // Arrange 
        Guid userId = Guid.NewGuid();
        Guid tipId = Guid.NewGuid();

        _userService.Setup(x => x.GetUserId())
            .Returns(userId);

        User user = new("username", "password");

        _userRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var service = CreateService();

        // Act
        await service.EditTipAsync(tipId, 10, "06/12/2023");

        // Assert
        _userService.Verify(x => x.GetUserId(), Times.Once);
        _userRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        _cache.Verify(x => x.RemoveAsync(
            $"user_info_{userId}",
            default
        ), Times.Once);
    }

    [Fact]
    public async Task UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid tipId = Guid.NewGuid();
        var service = CreateService();

        _userService.Setup(x => x.GetUserId())
            .Returns(userId);

        _userRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => service.EditTipAsync(tipId, 10, "06/12/2023"));

        // Assert
        Assert.Equal("User not found.", exception.Message);

        _userService.Verify(x => x.GetUserId(), Times.Once);
        _userRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        _cache.Verify(x => x.RemoveAsync(It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task InvalidTipInput_ThrowsInvalidTipAmountException()
    {
        // Arrange
        var service = CreateService();

        // Act
        var exception = await Assert.ThrowsAsync<InvalidTipAmountException>(() =>
            service.AddTipAsync(0)
        );

        // Assert
        Assert.Equal("Tip amount must be positive.", exception.Message);

        _userService.Verify(x => x.GetUserId(), Times.Never);
        _userRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _userRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        _cache.Verify(x => x.RemoveAsync(It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task InvalidDateInput_ThrowsInvalidDateFormatException()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid tipId = Guid.NewGuid();
        var service = CreateService();

        _userService.Setup(x => x.GetUserId())
            .Returns(userId);
        
        User user = new("username", "password");

        _userRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

         // Act
        var exception = await Assert.ThrowsAsync<InvalidDateFormatException>(() => 
            service.EditTipAsync(tipId, 10, "12-12-2023")
        );

        // Assert
        Assert.Equal("Date must be in MM/dd/yyyy format.", exception.Message);

        _userService.Verify(x => x.GetUserId(), Times.Once);
        _userRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        _cache.Verify(x => x.RemoveAsync(It.IsAny<string>(), default), Times.Never);
    }
}