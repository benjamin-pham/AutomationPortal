using Microsoft.EntityFrameworkCore;
using AutomationPortal.Domain.Entities;
using AutomationPortal.Domain.Repositories;
using AutomationPortal.Infrastructure.Data;

namespace AutomationPortal.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        await Context.Users
            .FirstOrDefaultAsync(u => u.Username == username, ct);

    public new async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await Context.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByHashedRefreshTokenAsync(string hashedToken, CancellationToken ct = default) =>
        await Context.Users
            .FirstOrDefaultAsync(u => u.HashedRefreshToken == hashedToken, ct);
}
