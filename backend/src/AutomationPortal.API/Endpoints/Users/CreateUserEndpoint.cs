using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutomationPortal.Application.Features.Users.CreateUser;
using AutomationPortal.Domain.Errors;

namespace AutomationPortal.API.Endpoints.Users;

internal sealed class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", async (
            CreateUserRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateUserCommand(
                request.FirstName,
                request.LastName,
                request.Username,
                request.Password,
                request.Email,
                request.Phone,
                request.Birthday);

            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
                return Results.Created($"/api/users/{result.Value.Id}", result.Value);

            if (result.Error == UserErrors.UsernameAlreadyExists)
                return Results.Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status409Conflict);

            return Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Description,
                statusCode: StatusCodes.Status422UnprocessableEntity);
        })
        .RequireAuthorization()
        .WithName("CreateUser")
        .WithTags("Users")
        .Produces<CreateUserResponse>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
        .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }
}

internal sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string Username,
    string Password,
    string? Email,
    string? Phone,
    DateOnly? Birthday);
