using HMS.Application.Abstractions.Persistence;
using HMS.Infrastructure.Persistence.Context;

namespace HMS.Infrastructure.Persistence.UnitOfWork;

internal sealed class EfUnitOfWork(HmsDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
