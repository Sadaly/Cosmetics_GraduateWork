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
            //Вызывается каждый раз при запуске теста
            _dbContext = dbContext;
            _transaction = _dbContext.Database.BeginTransaction();
        }

        //Работа этого метода на результат не влияет
        //так как для этого транзакция должна сохранятся,
        //а в конструктуре каждый раз создается новая
        public void Dispose()
        {
            //эта строка может сохранить транзакцию, но при этом ничего, кроме Dispose, нельзя сделать
            //_transaction.Commit();

            //эта строка не влияет на результат
            //_transaction.Rollback();
            
            _transaction.Dispose();
            _dbContext.Dispose();
        }
    }
}
