using Application.Entity.ReservedDates.Queries;
using Application.Entity.ReservedDates.Queries.Get;
using Application.UnitTests.TheoryData;
using Domain.Entity;
using Domain.Enums;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.SupportData.Filters;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace Application.UnitTests.Entities.ReservedDates.Queries
{
	public class ReservedDateGetQueryTests : TestsTheoryData
	{
		private readonly ReservedDateGetQueryHandler _handler;
		private readonly IReservedDateRepository _repository;
		private readonly ReservedDate _reserveddate;
		private readonly ReservedDateFilter _filter;

		public ReservedDateGetQueryTests()
		{
			_repository = Substitute.For<IReservedDateRepository>();
			_reserveddate = ReservedDate.Create(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1), ReservedDateEnumType.None).Value;
			_filter = new ReservedDateFilter();
			_handler = new ReservedDateGetQueryHandler(_repository);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<ReservedDate, bool>>>(expr =>
				expr.Compile()(_reserveddate) == true), Arg.Any<CancellationToken>())
				.Returns(_reserveddate);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<ReservedDate, bool>>>(expr =>
				expr.Compile()(_reserveddate) == false), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<ReservedDate>(PersistenceErrors.Entity<ReservedDate>.NotFound));
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new ReservedDateGetQuery(ReservedDateQueries.GetById(_reserveddate.Id)), default);

			//Assert
			result.Value.Type.Should().Be(_reserveddate.Type);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new ReservedDateGetQuery(ReservedDateQueries.GetById(Guid.Parse(id))), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<ReservedDate>.NotFound);
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidName()
		{
			//Act
			_filter.Type = "None";
			var result = await _handler.Handle(new ReservedDateGetQuery(ReservedDateQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Type.Should().Be(_reserveddate.Type);
		}
		[Theory]
		[MemberData(nameof(ValidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new ReservedDateGetQuery(ReservedDateQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Type.Should().Be(_reserveddate.Type);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new ReservedDateGetQuery(ReservedDateQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<ReservedDate>.NotFound);
		}
	}
}