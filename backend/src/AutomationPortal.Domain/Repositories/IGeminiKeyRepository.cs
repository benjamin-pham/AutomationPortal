using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Entities;

namespace AutomationPortal.Domain.Repositories;

public interface IGeminiKeyRepository : IRepository<GeminiKey>
{
    Task<GeminiKey?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<GeminiKey?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}
