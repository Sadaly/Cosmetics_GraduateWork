using Microsoft.EntityFrameworkCore.Storage;
using Persistence;

namespace WebApi.IntegrationTests
{
    public class TransactionalTestDatabase : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly IDbContextTransaction _transaction;

        public TransactionalTestDatabase(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _transaction = _dbContext.Database.BeginTransaction();
            _dbContext.Database.EnsureDeletedAsync();
        }

        public void Dispose()
        {
            _transaction.Rollback(); // ← Rolls back at test end
            _transaction.Dispose();
            _dbContext.Dispose();
        }
    }
}
