using AutomationPortal.Application.Abstractions.Messaging;

namespace AutomationPortal.Application.Features.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserDetailResponse>;
