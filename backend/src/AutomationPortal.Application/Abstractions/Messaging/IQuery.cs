using MediatR;
using AutomationPortal.Domain.Abstractions;

namespace AutomationPortal.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
