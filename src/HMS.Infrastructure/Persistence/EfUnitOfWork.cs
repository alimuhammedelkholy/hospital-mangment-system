using HMS.Application.Abstractions;

namespace HMS.Infrastructure.Persistence;

internal sealed class EfUnitOfWork(HmsDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
