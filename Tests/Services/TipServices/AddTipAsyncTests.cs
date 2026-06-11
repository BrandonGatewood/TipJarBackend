using Microsoft.Extensions.Caching.Distributed;
using Moq;
using TipJar.Application.Exceptions;
using TipJar.Application.Services;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Services;
using Xunit;

namespace TipJar.Tests.Services.TipServices;

public class AddTipAsyncTests
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
    public async Task ValidTip_SavesTip()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userService.Setup(x => x.GetUserId())
            .Returns(userId);

        var user = new User("username", "password"); // assuming default ctor works

        _userRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var service = CreateService();

        // Act
        await service.AddTipAsync(10);

        // Assert
        _tipRepository.Verify(x => x.AddAsync(It.IsAny<Tip>()), Times.Once);

        _userRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

        _cache.Verify(x => x.RemoveAsync(
            $"user_info_{userId}",
            default
        ), Times.Once);
    }

    [Fact]
    public async Task InvalidAmount_ThrowsInvalidTipAmountException()
    {
        // Arrange
        var service = CreateService();

        // Act
        var exception = await Assert.ThrowsAsync<InvalidTipAmountException>(() =>
            service.AddTipAsync(0)
        );

        // Assert
        Assert.Equal("Tip amount must be positive.", exception.Message);

        // Verify userService was never called
        _userService.Verify(x => x.GetUserId(), Times.Never);

        // Verify userRepository was never called
        _userRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _userRepository.Verify(x => x.SaveChangesAsync(), Times.Never);

        // verfiy tipRepository was never called
        _tipRepository.Verify(x => x.AddAsync(It.IsAny<Tip>()), Times.Never);

        // Verify cache was never called 
        _cache.Verify(x => x.RemoveAsync(It.IsAny<string>(), default), Times.Never);
    }
}