using Application.Entity.SkinCareTypes.Queries;
using Application.Entity.SkinCareTypes.Queries.Get;
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

namespace Application.UnitTests.Entities.SkinCareTypes.Queries
{
	public class SkinCareTypeGetQueryTests : TestsTheoryData
	{
		private readonly SkinCareTypeGetQueryHandler _handler;
		private readonly ISkinCareTypeRepository _repository;
		private readonly SkinCareType _skincaretype;
		private readonly SkinCareTypeFilter _filter;

		public SkinCareTypeGetQueryTests()
		{
			_repository = Substitute.For<ISkinCareTypeRepository>();
			_skincaretype = SkinCareType.Create(Title.Create("Fullname").Value).Value;
			_filter = new SkinCareTypeFilter();
			_handler = new SkinCareTypeGetQueryHandler(_repository);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<SkinCareType, bool>>>(expr =>
				expr.Compile()(_skincaretype) == true), Arg.Any<CancellationToken>())
				.Returns(_skincaretype);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<SkinCareType, bool>>>(expr =>
				expr.Compile()(_skincaretype) == false), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<SkinCareType>(PersistenceErrors.Entity<SkinCareType>.NotFound));
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new SkinCareTypeGetQuery(SkinCareTypeQueries.GetById(_skincaretype.Id)), default);

			//Assert
			result.Value.Id.Should().Be(_skincaretype.Id);
			result.Value.Title.Should().Be(_skincaretype.Title.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new SkinCareTypeGetQuery(SkinCareTypeQueries.GetById(Guid.Parse(id))), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<SkinCareType>.NotFound);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new SkinCareTypeGetQuery(SkinCareTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Id.Should().Be(_skincaretype.Id);
			result.Value.Title.Should().Be(_skincaretype.Title.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new SkinCareTypeGetQuery(SkinCareTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<SkinCareType>.NotFound);
		}
		[Theory]
		[MemberData(nameof(ValidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new SkinCareTypeGetQuery(SkinCareTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Id.Should().Be(_skincaretype.Id);
			result.Value.Title.Should().Be(_skincaretype.Title.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new SkinCareTypeGetQuery(SkinCareTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<SkinCareType>.NotFound);
		}
	}
}