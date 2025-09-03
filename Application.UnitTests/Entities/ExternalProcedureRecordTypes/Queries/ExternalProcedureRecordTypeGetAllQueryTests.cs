using Application.Entity.ExternalProcedureRecordTypes.Queries;
using Application.Entity.ExternalProcedureRecordTypes.Queries.GetAll;
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

namespace Application.UnitTests.Entities.ExternalProcedureRecordTypes.Queries
{
	public class ExternalProcedureRecordTypeGetAllQueryTests : TestsTheoryData
	{
		private readonly ExternalProcedureRecordTypeGetAllQueryHandler _handler;
		private readonly IExternalProcedureRecordTypeRepository _repository;
		private readonly ExternalProcedureRecordType _externalprocedurerecordtype1;
		private readonly ExternalProcedureRecordType _externalprocedurerecordtype2;
		private readonly ExternalProcedureRecordTypeFilter _filter;

		public ExternalProcedureRecordTypeGetAllQueryTests()
		{
			_repository = Substitute.For<IExternalProcedureRecordTypeRepository>();
			_externalprocedurerecordtype1 = ExternalProcedureRecordType.Create(Title.Create("Fullname1").Value).Value;
			_externalprocedurerecordtype2 = ExternalProcedureRecordType.Create(Title.Create("Fullname2").Value).Value;
			_filter = new ExternalProcedureRecordTypeFilter();
			_handler = new ExternalProcedureRecordTypeGetAllQueryHandler(_repository);

			_repository.GetAllAsync(Arg.Is<Expression<Func<ExternalProcedureRecordType, bool>>>(expr =>
				expr.Compile()(_externalprocedurerecordtype1) == true && expr.Compile()(_externalprocedurerecordtype2) == true), Arg.Any<CancellationToken>())
				.Returns(new List<ExternalProcedureRecordType>() { _externalprocedurerecordtype1, _externalprocedurerecordtype2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<ExternalProcedureRecordType, bool>>>(expr =>
				expr.Compile()(_externalprocedurerecordtype1) == true && expr.Compile()(_externalprocedurerecordtype2) == false), Arg.Any<CancellationToken>())
				.Returns(new List<ExternalProcedureRecordType>() { _externalprocedurerecordtype1 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<ExternalProcedureRecordType, bool>>>(expr =>
				expr.Compile()(_externalprocedurerecordtype2) == true && expr.Compile()(_externalprocedurerecordtype1) == false), Arg.Any<CancellationToken>())
				.Returns(new List<ExternalProcedureRecordType>() { _externalprocedurerecordtype2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<ExternalProcedureRecordType, bool>>>(expr =>
				expr.Compile()(_externalprocedurerecordtype1) == false && expr.Compile()(_externalprocedurerecordtype2) == false), Arg.Any<CancellationToken>())
				.Returns(new List<ExternalProcedureRecordType>() { });
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new ExternalProcedureRecordTypeGetAllQuery(ExternalProcedureRecordTypeQueries.GetWithoutPredicate()), default);

			//Assert
			result.Value[0].Id.Should().Be(_externalprocedurerecordtype1.Id);
			result.Value[1].Id.Should().Be(_externalprocedurerecordtype2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new ExternalProcedureRecordTypeGetAllQuery(ExternalProcedureRecordTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].Id.Should().Be(_externalprocedurerecordtype1.Id);
			result.Value[1].Id.Should().Be(_externalprocedurerecordtype2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidOnlyOneNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new ExternalProcedureRecordTypeGetAllQuery(ExternalProcedureRecordTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(1);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new ExternalProcedureRecordTypeGetAllQuery(ExternalProcedureRecordTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(0);
		}
		[Theory]
		[MemberData(nameof(ValidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new ExternalProcedureRecordTypeGetAllQuery(ExternalProcedureRecordTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].Id.Should().Be(_externalprocedurerecordtype1.Id);
			result.Value[1].Id.Should().Be(_externalprocedurerecordtype2.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new ExternalProcedureRecordTypeGetAllQuery(ExternalProcedureRecordTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(0);
		}

		[Theory]
		[MemberData(nameof(ValidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<ExternalProcedureRecordType, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(new List<ExternalProcedureRecordType>() { _externalprocedurerecordtype1, _externalprocedurerecordtype2 }.Skip(startIndex).Take(count).ToList());

			//Act
			var result = await _handler.Handle(new ExternalProcedureRecordTypeGetAllQuery(ExternalProcedureRecordTypeQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.Value.Count.Should().Be(count - startIndex);
		}
		[Theory]
		[MemberData(nameof(InvalidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<ExternalProcedureRecordType, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<ExternalProcedureRecordType>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<ExternalProcedureRecordType, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<ExternalProcedureRecordType>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<ExternalProcedureRecordType, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<ExternalProcedureRecordType>>(PersistenceErrors.IncorrectCount));

			//Act
			var result = await _handler.Handle(new ExternalProcedureRecordTypeGetAllQuery(ExternalProcedureRecordTypeQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.IsFailure.Should().BeTrue();
		}
	}
}