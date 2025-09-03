using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace WebApi.IntegrationTests
{
	public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
	{
		private readonly IServiceScope _scope;
		protected readonly ISender Sender;
		protected readonly AppDbContext dbContext;
		protected readonly TransactionalTestDatabase _transactionalTest;

		protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
		{
			_scope = factory.Services.CreateScope();
			Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

			dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
			_transactionalTest = _scope.ServiceProvider.GetRequiredService<TransactionalTestDatabase>();
		}

		public void Dispose()
		{
			//Вызывается после каждого теста, но эта строка не обязательно, т.к. транзакция не сохраняется (смотри комментарии в методе)
			_transactionalTest.Dispose();
		}
	}
}
