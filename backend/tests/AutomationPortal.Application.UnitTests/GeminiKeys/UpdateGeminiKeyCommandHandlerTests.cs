using AutomationPortal.Application.Features.GeminiKeys.UpdateGeminiKey;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Entities;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AutomationPortal.Application.UnitTests.GeminiKeys;

public sealed class UpdateGeminiKeyCommandHandlerTests
{
    private readonly IGeminiKeyRepository _geminiKeyRepository = Substitute.For<IGeminiKeyRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<UpdateGeminiKeyCommandHandler> _logger = Substitute.For<ILogger<UpdateGeminiKeyCommandHandler>>();
    private readonly UpdateGeminiKeyCommandHandler _handler;

    public UpdateGeminiKeyCommandHandlerTests()
    {
        _handler = new UpdateGeminiKeyCommandHandler(_geminiKeyRepository, _unitOfWork, _logger);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var keyId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var existingKey = new GeminiKey
        {
            Id = keyId,
            Name = "Old Name",
            KeyValue = "OldKeyValue",
            UserId = originalUserId
        };

        var command = new UpdateGeminiKeyCommand(keyId, "New Name", "NewValue123", newUserId);
        
        _geminiKeyRepository.GetByIdAsync(keyId, Arg.Any<CancellationToken>()).Returns(existingKey);
        _geminiKeyRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns((GeminiKey?)null);
        _geminiKeyRepository.GetByUserIdAsync(command.UserId, Arg.Any<CancellationToken>()).Returns((GeminiKey?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingKey.Name.Should().Be(command.Name);
        existingKey.KeyValue.Should().Be(command.KeyValue);
        existingKey.UserId.Should().Be(command.UserId);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenKeyNotFound()
    {
        // Arrange
        var command = new UpdateGeminiKeyCommand(Guid.NewGuid(), "Name", "Value", Guid.NewGuid());
        _geminiKeyRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((GeminiKey?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeminiKeyErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenNameAlreadyExistsOnOtherRecord()
    {
        // Arrange
        var keyId = Guid.NewGuid();
        var otherId = Guid.NewGuid();
        var existingKey = new GeminiKey
        {
            Id = keyId,
            Name = "My Key",
            KeyValue = "Value",
            UserId = Guid.NewGuid()
        };
        
        var otherKey = new GeminiKey
        {
            Id = otherId,
            Name = "Duplicate Name",
            KeyValue = "OtherValue",
            UserId = Guid.NewGuid()
        };

        var command = new UpdateGeminiKeyCommand(keyId, "Duplicate Name", "Value", Guid.NewGuid());
        
        _geminiKeyRepository.GetByIdAsync(keyId, Arg.Any<CancellationToken>()).Returns(existingKey);
        _geminiKeyRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns(otherKey);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeminiKeyErrors.NameAlreadyExists);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserAlreadyHasKeyAndNotReplacing()
    {
        // Arrange
        var keyId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var existingKey = new GeminiKey
        {
            Id = keyId,
            Name = "My Key",
            KeyValue = "Value",
            UserId = Guid.NewGuid()
        };
        
        var otherKeyForTargetUser = new GeminiKey
        {
            Id = Guid.NewGuid(),
            Name = "Other Key",
            KeyValue = "OtherValue",
            UserId = otherUserId
        };

        var command = new UpdateGeminiKeyCommand(keyId, "My Key", "Value", otherUserId, ReplaceExisting: false);
        
        _geminiKeyRepository.GetByIdAsync(keyId, Arg.Any<CancellationToken>()).Returns(existingKey);
        _geminiKeyRepository.GetByUserIdAsync(otherUserId, Arg.Any<CancellationToken>()).Returns(otherKeyForTargetUser);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeminiKeyErrors.UserAlreadyHasKey);
    }

    [Fact]
    public async Task Handle_Should_DeleteOtherKeyAndProceed_WhenUserAlreadyHasKeyAndReplacing()
    {
        // Arrange
        var keyId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var existingKey = new GeminiKey
        {
            Id = keyId,
            Name = "My Key",
            KeyValue = "Value",
            UserId = Guid.NewGuid()
        };
        
        var otherKeyForTargetUser = new GeminiKey
        {
            Id = Guid.NewGuid(),
            Name = "Other Key",
            KeyValue = "OtherValue",
            UserId = otherUserId
        };

        var command = new UpdateGeminiKeyCommand(keyId, "My Key", "Value", otherUserId, ReplaceExisting: true);
        
        _geminiKeyRepository.GetByIdAsync(keyId, Arg.Any<CancellationToken>()).Returns(existingKey);
        _geminiKeyRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns(existingKey); // self
        _geminiKeyRepository.GetByUserIdAsync(otherUserId, Arg.Any<CancellationToken>()).Returns(otherKeyForTargetUser);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _geminiKeyRepository.Received(1).Remove(otherKeyForTargetUser);
        existingKey.UserId.Should().Be(otherUserId);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_KeepKeyValue_WhenValueIsMasked()
    {
        // Arrange
        var keyId = Guid.NewGuid();
        var originalValue = "SUPER_SECRET_KEY";
        var userId = Guid.NewGuid();
        var existingKey = new GeminiKey
        {
            Id = keyId,
            Name = "My Key",
            KeyValue = originalValue,
            UserId = userId
        };

        // Frontend sends masked value back if not changed
        var command = new UpdateGeminiKeyCommand(keyId, "Updated Name", "****abcd", userId);
        
        _geminiKeyRepository.GetByIdAsync(keyId, Arg.Any<CancellationToken>()).Returns(existingKey);
        _geminiKeyRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns((GeminiKey?)null);
        _geminiKeyRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingKey);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingKey.Name.Should().Be(command.Name);
        existingKey.KeyValue.Should().Be(originalValue); // Should NOT be ****abcd
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_KeepKeyValue_WhenValueIsNull()
    {
        // Arrange
        var keyId = Guid.NewGuid();
        var originalValue = "SUPER_SECRET_KEY";
        var userId = Guid.NewGuid();
        var existingKey = new GeminiKey
        {
            Id = keyId,
            Name = "My Key",
            KeyValue = originalValue,
            UserId = userId
        };

        // Frontend sends null if not provided
        var command = new UpdateGeminiKeyCommand(keyId, "Updated Name", null, userId);
        
        _geminiKeyRepository.GetByIdAsync(keyId, Arg.Any<CancellationToken>()).Returns(existingKey);
        _geminiKeyRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns((GeminiKey?)null);
        _geminiKeyRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingKey);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingKey.Name.Should().Be(command.Name);
        existingKey.KeyValue.Should().Be(originalValue);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
