using MediatR;
using AutomationPortal.Domain.Abstractions;

namespace AutomationPortal.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
