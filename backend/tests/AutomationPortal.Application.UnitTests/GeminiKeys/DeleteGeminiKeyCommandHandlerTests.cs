using AutomationPortal.Application.Features.GeminiKeys.DeleteGeminiKey;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Entities;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AutomationPortal.Application.UnitTests.GeminiKeys;

public sealed class DeleteGeminiKeyCommandHandlerTests
{
    private readonly IGeminiKeyRepository _geminiKeyRepository = Substitute.For<IGeminiKeyRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<DeleteGeminiKeyCommandHandler> _logger = Substitute.For<ILogger<DeleteGeminiKeyCommandHandler>>();
    private readonly DeleteGeminiKeyCommandHandler _handler;

    public DeleteGeminiKeyCommandHandlerTests()
    {
        _handler = new DeleteGeminiKeyCommandHandler(_geminiKeyRepository, _unitOfWork, _logger);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenGeminiKeyExists()
    {
        // Arrange
        var geminiKeyId = Guid.NewGuid();
        var geminiKey = GeminiKey.Create("Test Key", "Value123", Guid.NewGuid());
        var command = new DeleteGeminiKeyCommand(geminiKeyId);

        _geminiKeyRepository.GetByIdAsync(geminiKeyId, Arg.Any<CancellationToken>()).Returns(geminiKey);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _geminiKeyRepository.Received(1).Remove(geminiKey);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenGeminiKeyDoesNotExist()
    {
        // Arrange
        var geminiKeyId = Guid.NewGuid();
        var command = new DeleteGeminiKeyCommand(geminiKeyId);

        _geminiKeyRepository.GetByIdAsync(geminiKeyId, Arg.Any<CancellationToken>()).Returns((GeminiKey?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeminiKeyErrors.NotFound);
        _geminiKeyRepository.DidNotReceive().Remove(Arg.Any<GeminiKey>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
