using Application.Entity.PatientSpecificses.Queries;
using Application.Entity.PatientSpecificses.Queries.GetAll;
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

namespace Application.UnitTests.Entities.PatientSpecificss.Queries
{
	public class PatientSpecificsGetAllQueryTests : TestsTheoryData
	{
		private readonly PatientSpecificsGetAllQueryHandler _handler;
		private readonly IPatientSpecificsRepository _repository;
		private readonly PatientSpecifics _patientspecifics1;
		private readonly PatientSpecifics _patientspecifics2;
		private readonly PatientSpecificsFilter _filter;

		public PatientSpecificsGetAllQueryTests()
		{
			_repository = Substitute.For<IPatientSpecificsRepository>();
			_patientspecifics1 = PatientSpecifics.Create("", "", "", "", Patient.Create(Username.Create("Fullname1")).Value.Card).Value;
			_patientspecifics2 = PatientSpecifics.Create("", "", "", "", Patient.Create(Username.Create("Fullname2")).Value.Card).Value;
			_filter = new PatientSpecificsFilter();
			_handler = new PatientSpecificsGetAllQueryHandler(_repository);

			_repository.GetAllAsync(Arg.Is<Expression<Func<PatientSpecifics, bool>>>(expr =>
				expr.Compile()(_patientspecifics1) == true && expr.Compile()(_patientspecifics2) == true), Arg.Any<CancellationToken>())
				.Returns(new List<PatientSpecifics>() { _patientspecifics1, _patientspecifics2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<PatientSpecifics, bool>>>(expr =>
				expr.Compile()(_patientspecifics1) == true && expr.Compile()(_patientspecifics2) == false), Arg.Any<CancellationToken>())
				.Returns(new List<PatientSpecifics>() { _patientspecifics1 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<PatientSpecifics, bool>>>(expr =>
				expr.Compile()(_patientspecifics2) == true && expr.Compile()(_patientspecifics1) == false), Arg.Any<CancellationToken>())
				.Returns(new List<PatientSpecifics>() { _patientspecifics2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<PatientSpecifics, bool>>>(expr =>
				expr.Compile()(_patientspecifics1) == false && expr.Compile()(_patientspecifics2) == false), Arg.Any<CancellationToken>())
				.Returns(new List<PatientSpecifics>() { });
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetWithoutPredicate()), default);

			//Assert
			result.Value[0].Id.Should().Be(_patientspecifics1.Id);
			result.Value[1].Id.Should().Be(_patientspecifics2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].Id.Should().Be(_patientspecifics1.Id);
			result.Value[1].Id.Should().Be(_patientspecifics2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidOnlyOneNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(1);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(_filter)), default);

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
			var result = await _handler.Handle(new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].Id.Should().Be(_patientspecifics1.Id);
			result.Value[1].Id.Should().Be(_patientspecifics2.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(0);
		}

		[Theory]
		[MemberData(nameof(ValidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<PatientSpecifics, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(new List<PatientSpecifics>() { _patientspecifics1, _patientspecifics2 }.Skip(startIndex).Take(count).ToList());

			//Act
			var result = await _handler.Handle(new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.Value.Count.Should().Be(count - startIndex);
		}

		[Theory]
		[MemberData(nameof(InvalidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<PatientSpecifics, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<PatientSpecifics>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<PatientSpecifics, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<PatientSpecifics>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<PatientSpecifics, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<PatientSpecifics>>(PersistenceErrors.IncorrectCount));

			//Act
			var result = await _handler.Handle(new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.IsFailure.Should().BeTrue();
		}
	}
}