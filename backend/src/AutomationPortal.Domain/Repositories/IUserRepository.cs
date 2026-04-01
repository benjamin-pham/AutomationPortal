using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Entities;

namespace AutomationPortal.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    new Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByHashedRefreshTokenAsync(string hashedToken, CancellationToken ct = default);
}
