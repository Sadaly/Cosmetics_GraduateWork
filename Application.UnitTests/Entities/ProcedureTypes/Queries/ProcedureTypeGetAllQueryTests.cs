using Application.Entity.ProcedureTypes.Queries;
using Application.Entity.ProcedureTypes.Queries.GetAll;
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

namespace Application.UnitTests.Entities.ProcedureTypes.Queries
{
	public class ProcedureTypeGetAllQueryTests : TestsTheoryData
	{
		private readonly ProcedureTypeGetAllQueryHandler _handler;
		private readonly IProcedureTypeRepository _repository;
		private readonly ProcedureType _proceduretype1;
		private readonly ProcedureType _proceduretype2;
		private readonly ProcedureTypeFilter _filter;

		public ProcedureTypeGetAllQueryTests()
		{
			_repository = Substitute.For<IProcedureTypeRepository>();
			_proceduretype1 = ProcedureType.Create(Title.Create("Fullname1").Value, "", 0).Value;
			_proceduretype2 = ProcedureType.Create(Title.Create("Fullname2").Value, "", 0).Value;
			_filter = new ProcedureTypeFilter();
			_handler = new ProcedureTypeGetAllQueryHandler(_repository);

			_repository.GetAllAsync(Arg.Is<Expression<Func<ProcedureType, bool>>>(expr =>
				expr.Compile()(_proceduretype1) == true && expr.Compile()(_proceduretype2) == true), Arg.Any<CancellationToken>())
				.Returns(new List<ProcedureType>() { _proceduretype1, _proceduretype2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<ProcedureType, bool>>>(expr =>
				expr.Compile()(_proceduretype1) == true && expr.Compile()(_proceduretype2) == false), Arg.Any<CancellationToken>())
				.Returns(new List<ProcedureType>() { _proceduretype1 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<ProcedureType, bool>>>(expr =>
				expr.Compile()(_proceduretype2) == true && expr.Compile()(_proceduretype1) == false), Arg.Any<CancellationToken>())
				.Returns(new List<ProcedureType>() { _proceduretype2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<ProcedureType, bool>>>(expr =>
				expr.Compile()(_proceduretype1) == false && expr.Compile()(_proceduretype2) == false), Arg.Any<CancellationToken>())
				.Returns(new List<ProcedureType>() { });
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new ProcedureTypeGetAllQuery(ProcedureTypeQueries.GetWithoutPredicate()), default);

			//Assert
			result.Value[0].Id.Should().Be(_proceduretype1.Id);
			result.Value[1].Id.Should().Be(_proceduretype2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new ProcedureTypeGetAllQuery(ProcedureTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].Id.Should().Be(_proceduretype1.Id);
			result.Value[1].Id.Should().Be(_proceduretype2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidOnlyOneNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new ProcedureTypeGetAllQuery(ProcedureTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(1);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new ProcedureTypeGetAllQuery(ProcedureTypeQueries.GetByFilter(_filter)), default);

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
			var result = await _handler.Handle(new ProcedureTypeGetAllQuery(ProcedureTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].Id.Should().Be(_proceduretype1.Id);
			result.Value[1].Id.Should().Be(_proceduretype2.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new ProcedureTypeGetAllQuery(ProcedureTypeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(0);
		}

		[Theory]
		[MemberData(nameof(ValidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<ProcedureType, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(new List<ProcedureType>() { _proceduretype1, _proceduretype2 }.Skip(startIndex).Take(count).ToList());

			//Act
			var result = await _handler.Handle(new ProcedureTypeGetAllQuery(ProcedureTypeQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.Value.Count.Should().Be(count - startIndex);
		}
		[Theory]
		[MemberData(nameof(InvalidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<ProcedureType, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<ProcedureType>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<ProcedureType, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<ProcedureType>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<ProcedureType, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<ProcedureType>>(PersistenceErrors.IncorrectCount));

			//Act
			var result = await _handler.Handle(new ProcedureTypeGetAllQuery(ProcedureTypeQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.IsFailure.Should().BeTrue();
		}
	}
}