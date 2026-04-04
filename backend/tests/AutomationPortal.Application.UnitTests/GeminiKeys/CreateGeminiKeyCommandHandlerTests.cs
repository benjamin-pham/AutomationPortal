using AutomationPortal.Application.Features.GeminiKeys.CreateGeminiKey;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Entities;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AutomationPortal.Application.UnitTests.GeminiKeys;

public sealed class CreateGeminiKeyCommandHandlerTests
{
    private readonly IGeminiKeyRepository _geminiKeyRepository = Substitute.For<IGeminiKeyRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<CreateGeminiKeyCommandHandler> _logger = Substitute.For<ILogger<CreateGeminiKeyCommandHandler>>();
    private readonly CreateGeminiKeyCommandHandler _handler;

    public CreateGeminiKeyCommandHandlerTests()
    {
        _handler = new CreateGeminiKeyCommandHandler(_geminiKeyRepository, _unitOfWork, _logger);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateGeminiKeyCommand("Test Key", "Value123", Guid.NewGuid());
        _geminiKeyRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns((GeminiKey?)null);
        _geminiKeyRepository.GetByUserIdAsync(command.UserId, Arg.Any<CancellationToken>()).Returns((GeminiKey?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(command.Name);
        _geminiKeyRepository.Received(1).Add(Arg.Is<GeminiKey>(k => k.Name == command.Name));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenNameAlreadyExists()
    {
        // Arrange
        var command = new CreateGeminiKeyCommand("Existing Name", "Value123", Guid.NewGuid());
        var existingKey = GeminiKey.Create("Existing Name", "OtherValue", Guid.NewGuid());
        _geminiKeyRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns(existingKey);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeminiKeyErrors.NameAlreadyExists);
        _geminiKeyRepository.DidNotReceive().Add(Arg.Any<GeminiKey>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserAlreadyHasKey_AndNotReplacing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateGeminiKeyCommand("New Name", "Value123", userId, ReplaceExisting: false);
        var existingKey = GeminiKey.Create("Old Name", "OldValue", userId);
        
        _geminiKeyRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns((GeminiKey?)null);
        _geminiKeyRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingKey);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeminiKeyErrors.UserAlreadyHasKey);
        _geminiKeyRepository.DidNotReceive().Add(Arg.Any<GeminiKey>());
    }

    [Fact]
    public async Task Handle_Should_DeleteExistingKeyAndCreateNew_WhenUserAlreadyHasKey_AndReplacing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateGeminiKeyCommand("New Name", "Value123", userId, ReplaceExisting: true);
        var existingKey = GeminiKey.Create("Old Name", "OldValue", userId);
        
        _geminiKeyRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns((GeminiKey?)null);
        _geminiKeyRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingKey);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _geminiKeyRepository.Received(1).Remove(existingKey);
        _geminiKeyRepository.Received(1).Add(Arg.Is<GeminiKey>(k => k.Name == command.Name));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
