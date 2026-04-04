using Microsoft.EntityFrameworkCore;
using AutomationPortal.Domain.Entities;
using AutomationPortal.Domain.Repositories;
using AutomationPortal.Infrastructure.Data;

namespace AutomationPortal.Infrastructure.Repositories;

public sealed class GeminiKeyRepository(AppDbContext context) : Repository<GeminiKey>(context), IGeminiKeyRepository
{
    public async Task<GeminiKey?> GetByNameAsync(string name, CancellationToken ct = default) =>
        await Context.GeminiKeys
            .FirstOrDefaultAsync(g => g.Name == name, ct);

    public async Task<GeminiKey?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await Context.GeminiKeys
            .FirstOrDefaultAsync(g => g.UserId == userId, ct);
}
