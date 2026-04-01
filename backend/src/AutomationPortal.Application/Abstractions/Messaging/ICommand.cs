using MediatR;
using AutomationPortal.Domain.Abstractions;

namespace AutomationPortal.Application.Abstractions.Messaging;

public interface IBaseCommand;

public interface ICommand : IRequest<Result>, IBaseCommand
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}
