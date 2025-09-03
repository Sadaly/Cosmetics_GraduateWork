using Application.Entity.HealthCondTypes.Queries;
using Application.Entity.HealthCondTypes.Queries.Get;
using Application.UnitTests.TheoryData;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.SupportData.Filters;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace Application.UnitTests.Entities.HealthCondTypes.Queries
{
	public class HealthCondTypeGetQueryTests : TestsTheoryData
	{
		private readonly HealthCondTypeGetQueryHandler _handler;
		private readonly IHealthCondTypeRepository _repository;
		private readonly HealthCondType _healthcondtype;
		private readonly HealthCondTypeFilter _filter;

		public HealthCondTypeGetQueryTests()
		{
			_repository = Substitute.For<IHealthCondTypeRepository>();
			_healthcondtype = HealthCondType.Create(Title.Create("Fullname").Value).Value;
			_filter = new HealthCondTypeFilter();
			_handler = new HealthCondTypeGetQueryHandler(_repository);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<HealthCondType, bool>>>(expr =>
				expr.Compile()(_healthcondtype) == true), Arg.Any<CancellationToken>())
				.Returns(_healthcondtype);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<HealthCondType, bool>>>(expr =>
				expr.Compile()(_healthcondtype) == false), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<HealthCondType>(PersistenceErrors.Entity<HealthCondType>.NotFound));
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new HealthCondTypeGetQuery(HealthCondTypeQueries.GetById(_healthcondtype.Id)), default);

			//Assert
			result.Value.Id.Should().Be(_healthcondtype.Id);
			result.Value.Title.Should().Be(_healthcondtype.Title.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new HealthCondTypeGetQuery(HealthCondTypeQueries.GetById(Guid.Parse(id))), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<HealthCondType>.NotFound);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new HealthCondTypeGetQuery(HealthCondTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Id.Should().Be(_healthcondtype.Id);
			result.Value.Title.Should().Be(_healthcondtype.Title.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new HealthCondTypeGetQuery(HealthCondTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<HealthCondType>.NotFound);
		}
		[Theory]
		[MemberData(nameof(ValidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new HealthCondTypeGetQuery(HealthCondTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Id.Should().Be(_healthcondtype.Id);
			result.Value.Title.Should().Be(_healthcondtype.Title.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new HealthCondTypeGetQuery(HealthCondTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<HealthCondType>.NotFound);
		}
	}
}