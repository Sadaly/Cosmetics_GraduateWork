using Domain.Entity;
using Domain.Repositories;
using Persistence.Abstractions;

namespace Persistence.Repositories
{
    public class ReservedDateRepository(AppDbContext dbContext)
        : TRepository<ReservedDate>(dbContext), IReservedDateRepository
    {
    }
}
