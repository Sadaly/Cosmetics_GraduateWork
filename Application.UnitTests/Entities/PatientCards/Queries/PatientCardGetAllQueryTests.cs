using Application.Entity.PatientCards.Queries;
using Application.Entity.PatientCards.Queries.GetAll;
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

namespace Application.UnitTests.Entities.PatientCards.Queries
{
	public class PatientCardGetAllQueryTests : TestsTheoryData
	{
		private readonly PatientCardGetAllQueryHandler _handler;
		private readonly IPatientCardRepository _repository;
		private readonly PatientCard _patientcard1;
		private readonly PatientCard _patientcard2;
		private readonly PatientCardFilter _filter;

		public PatientCardGetAllQueryTests()
		{
			_repository = Substitute.For<IPatientCardRepository>();
			_patientcard1 = Patient.Create(Username.Create("Fullname1")).Value.Card;
			_patientcard2 = Patient.Create(Username.Create("Fullname2")).Value.Card;
			_filter = new PatientCardFilter();
			_handler = new PatientCardGetAllQueryHandler(_repository);

			_repository.GetAllAsync(Arg.Is<Expression<Func<PatientCard, bool>>>(expr =>
				expr.Compile()(_patientcard1) == true && expr.Compile()(_patientcard2) == true), Arg.Any<CancellationToken>())
				.Returns(new List<PatientCard>() { _patientcard1, _patientcard2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<PatientCard, bool>>>(expr =>
				expr.Compile()(_patientcard1) == true && expr.Compile()(_patientcard2) == false), Arg.Any<CancellationToken>())
				.Returns(new List<PatientCard>() { _patientcard1 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<PatientCard, bool>>>(expr =>
				expr.Compile()(_patientcard2) == true && expr.Compile()(_patientcard1) == false), Arg.Any<CancellationToken>())
				.Returns(new List<PatientCard>() { _patientcard2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<PatientCard, bool>>>(expr =>
				expr.Compile()(_patientcard1) == false && expr.Compile()(_patientcard2) == false), Arg.Any<CancellationToken>())
				.Returns(new List<PatientCard>() { });
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new PatientCardGetAllQuery(PatientCardQueries.GetWithoutPredicate()), default);

			//Assert
			result.Value[0].Id.Should().Be(_patientcard1.Id);
			result.Value[1].Id.Should().Be(_patientcard2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].Id.Should().Be(_patientcard1.Id);
			result.Value[1].Id.Should().Be(_patientcard2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidOnlyOneNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(1);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(_filter)), default);

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
			var result = await _handler.Handle(new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].Id.Should().Be(_patientcard1.Id);
			result.Value[1].Id.Should().Be(_patientcard2.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(0);
		}

		[Theory]
		[MemberData(nameof(ValidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<PatientCard, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(new List<PatientCard>() { _patientcard1, _patientcard2 }.Skip(startIndex).Take(count).ToList());

			//Act
			var result = await _handler.Handle(new PatientCardGetAllQuery(PatientCardQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.Value.Count.Should().Be(count - startIndex);
		}
		[Theory]
		[MemberData(nameof(InvalidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<PatientCard, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<PatientCard>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<PatientCard, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<PatientCard>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<PatientCard, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<PatientCard>>(PersistenceErrors.IncorrectCount));

			//Act
			var result = await _handler.Handle(new PatientCardGetAllQuery(PatientCardQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.IsFailure.Should().BeTrue();
		}
	}
}