using Application.Entity.ReservedDates.Commands.Create;
using Application.Entity.ReservedDates.Commands.SoftDelete;
using Application.Entity.ReservedDates.Queries;
using Application.Entity.ReservedDates.Queries.Get;
using Application.Entity.ReservedDates.Queries.GetAll;
using Domain.SupportData.Filters;

namespace WebApi.IntegrationTests.Tests
{
	public class ReservedDateTests : BaseIntegrationTest
	{
		private readonly ReservedDateCreateCommand create;
		private readonly ReservedDateCreateCommand create1;
		private readonly ReservedDateCreateCommand create2;
		public ReservedDateTests(IntegrationTestWebAppFactory factory) : base(factory)
		{
			create = new(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), Domain.Enums.ReservedDateType.HolidayRestrict);
			create1 = new(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), Domain.Enums.ReservedDateType.DayOfWeekRestrict);
			create2 = new(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3), Domain.Enums.ReservedDateType.DayOfWeekRestrict);
		}

		[Fact]
		public async Task Create_ShouldAddReservedDate_WhenCorrectInput()
		{
			//Arrange

			//Act
			var id = await Sender.Send(create);

			//Assert
			Assert.True(dbContext.ReservedDates.FirstOrDefault(d => d.Id == id.Value) != null);
		}

		[Fact]
		public async Task RemoveById_ShouldRemoveReservedDate_WhenExists()
		{
			//Arrange
			var id = await Sender.Send(create);
			var command = new ReservedDateSoftDeleteCommand(id.Value);

			//Act
			var remove = await Sender.Send(command);
			var query = new ReservedDateGetQuery(ReservedDateQueries.GetById(id.Value));
			var result = await Sender.Send(query);

			//Assert
			Assert.True(result.IsFailure);
		}

		[Fact]
		public async Task RemoveById_ShouldReturnFailer_WhenNotExists()
		{
			//Arrange
			var command = new ReservedDateSoftDeleteCommand(Guid.NewGuid());

			//Act
			var result = await Sender.Send(command);

			//Assert
			Assert.True(result.IsFailure);
		}
		[Fact]
		public async Task GetById_ShouldReturnReservedDate_WhenExists()
		{
			//Arrange
			var id = await Sender.Send(create);
			var query = new ReservedDateGetQuery(ReservedDateQueries.GetById(id.Value));

			//Act
			var result = await Sender.Send(query);

			//Assert
			Assert.True(result.Value.Type == create.Type);
		}

		[Fact]
		public async Task GetById_ShouldReturnFailer_WhenNotExists()
		{
			//Arrange
			var query = new ReservedDateGetQuery(ReservedDateQueries.GetById(Guid.NewGuid()));

			//Act
			var result = await Sender.Send(query);

			//Assert
			Assert.True(result.IsFailure);
		}

		[Theory]
		[MemberData(nameof(CorrectFilterResults))]
		public async Task GetAllByFilter_ShouldReturnReservedDate_WhenExists(DateTime? creationDateFrom, DateTime? creationDateTo)
		{
			//Arrange
			var id1 = await Sender.Send(create1);
			var id2 = await Sender.Send(create2);

			var filter = new ReservedDateFilter()
			{
				CreationDateFrom = creationDateFrom,
				CreationDateTo = creationDateTo,
			};
			var query = new ReservedDateGetAllQuery(ReservedDateQueries.GetByFilter(filter));

			//Act
			var result = await Sender.Send(query);

			//Assert
			Assert.True(result.Value.Count == 2);
		}

		[Theory]
		[MemberData(nameof(IncorrectFilterResults))]
		public async Task GetAllByFilter_ShouldReturnNone_WhenNotExists(DateTime? creationDateFrom, DateTime? creationDateTo)
		{
			//Arrange
			var id1 = await Sender.Send(create1);
			var id2 = await Sender.Send(create2);

			var filter = new ReservedDateFilter()
			{
				CreationDateFrom = creationDateFrom,
				CreationDateTo = creationDateTo,
			};
			var query = new ReservedDateGetAllQuery(ReservedDateQueries.GetByFilter(filter));

			//Act
			var result = await Sender.Send(query);

			//Assert
			Assert.True(result.Value.Count == 0);
		}
		public static TheoryData<DateTime?, DateTime?> CorrectFilterResults = new()
		{
			{ null, null },
			{ null, DateTime.UtcNow.AddDays(1) },
			{ DateTime.UtcNow.AddDays(-1), null },
			{ DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1) },
		};
		public static TheoryData<DateTime?, DateTime?> IncorrectFilterResults = new()
		{
			{ null, DateTime.UtcNow.AddDays(-1) },
			{ DateTime.UtcNow.AddDays(1), null },
			{ DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(-1) },
		};



		[Theory]
		[MemberData(nameof(CorrectTakeDataResults))]
		public async Task Take_ShouldReturnReservedDate_WhenCorrectIndexes(int startIndex, int count)
		{
			//Arrange
			var id1 = await Sender.Send(create1);
			var id2 = await Sender.Send(create2);

			var filter = new ReservedDateFilter();

			var query = new ReservedDateGetAllQuery(ReservedDateQueries.GetByFilter(filter), startIndex, count);

			//Act
			var result = await Sender.Send(query);

			//Assert
			Assert.True(result.Value.Count == 1);
		}

		[Theory]
		[MemberData(nameof(IncorrectTakeDataResults))]
		public async Task Take_ShouldReturnNone_WhenIncorrectIndexes(int startIndex, int count)
		{
			//Arrange
			var id1 = await Sender.Send(create1);
			var id2 = await Sender.Send(create2);

			var filter = new ReservedDateFilter();
			var query = new ReservedDateGetAllQuery(ReservedDateQueries.GetByFilter(filter), startIndex, count);

			//Act
			var result = await Sender.Send(query);

			//Assert
			Assert.True(result.IsFailure || result.Value.Count == 0);
		}
		public static TheoryData<int, int> CorrectTakeDataResults = new()
		{
			{ 0, 1 },
			{ 1, 1 },
		};
		public static TheoryData<int, int> IncorrectTakeDataResults = new()
		{
			{ -1, 1 },
			{ 1, 0 },
			{ 2, 1 },
		};
	}
}