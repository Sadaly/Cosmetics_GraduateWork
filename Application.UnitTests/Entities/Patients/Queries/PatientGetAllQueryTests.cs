using Application.Entity.Patients.Queries;
using Application.Entity.Patients.Queries.GetAll;
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

namespace Application.UnitTests.Entities.Patients.Queries
{
	public class PatientGetAllQueryTests : TestsTheoryData
	{
		private readonly PatientGetAllQueryHandler _handler;
		private readonly IPatientRepository _repository;
		private readonly Patient _patient1;
		private readonly Patient _patient2;
		private readonly PatientFilter _filter;

		public PatientGetAllQueryTests()
		{
			_repository = Substitute.For<IPatientRepository>();
			_patient1 = Patient.Create(Username.Create("Fullname1").Value).Value;
			_patient2 = Patient.Create(Username.Create("Fullname2").Value).Value;
			_filter = new PatientFilter();
			_handler = new PatientGetAllQueryHandler(_repository);

			_repository.GetAllAsync(Arg.Is<Expression<Func<Patient, bool>>>(expr =>
				expr.Compile()(_patient1) == true && expr.Compile()(_patient2) == true), Arg.Any<CancellationToken>(), Domain.Abstractions.FetchMode.Include)
				.Returns(new List<Patient>() { _patient1, _patient2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<Patient, bool>>>(expr =>
				expr.Compile()(_patient1) == true && expr.Compile()(_patient2) == false), Arg.Any<CancellationToken>(), Domain.Abstractions.FetchMode.Include)
				.Returns(new List<Patient>() { _patient1 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<Patient, bool>>>(expr =>
				expr.Compile()(_patient2) == true && expr.Compile()(_patient1) == false), Arg.Any<CancellationToken>(), Domain.Abstractions.FetchMode.Include)
				.Returns(new List<Patient>() { _patient2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<Patient, bool>>>(expr =>
				expr.Compile()(_patient1) == false && expr.Compile()(_patient2) == false), Arg.Any<CancellationToken>(), Domain.Abstractions.FetchMode.Include)
				.Returns(new List<Patient>() { });
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new PatientGetAllQuery(PatientQueries.GetWithoutPredicate()), default);

			//Assert
			result.Value[0].PatientId.Should().Be(_patient1.Id);
			result.Value[1].PatientId.Should().Be(_patient2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.Fullname = name;
			var result = await _handler.Handle(new PatientGetAllQuery(PatientQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].PatientId.Should().Be(_patient1.Id);
			result.Value[1].PatientId.Should().Be(_patient2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidOnlyOneNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
		{
			//Act
			_filter.Fullname = name;
			var result = await _handler.Handle(new PatientGetAllQuery(PatientQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(1);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
		{
			//Act
			_filter.Fullname = name;
			var result = await _handler.Handle(new PatientGetAllQuery(PatientQueries.GetByFilter(_filter)), default);

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
			var result = await _handler.Handle(new PatientGetAllQuery(PatientQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].PatientId.Should().Be(_patient1.Id);
			result.Value[1].PatientId.Should().Be(_patient2.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new PatientGetAllQuery(PatientQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(0);
		}

		[Theory]
		[MemberData(nameof(ValidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<Patient, bool>>>(), Arg.Any<CancellationToken>(), Domain.Abstractions.FetchMode.Include)
				.Returns(new List<Patient>() { _patient1, _patient2 }.Skip(startIndex).Take(count).ToList());

			//Act
			var result = await _handler.Handle(new PatientGetAllQuery(PatientQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.Value.Count.Should().Be(count - startIndex);
		}
		[Theory]
		[MemberData(nameof(InvalidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<Patient, bool>>>(), Arg.Any<CancellationToken>(), Domain.Abstractions.FetchMode.Include)
				.Returns(Result.Failure<List<Patient>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<Patient, bool>>>(), Arg.Any<CancellationToken>(), Domain.Abstractions.FetchMode.Include)
				.Returns(Result.Failure<List<Patient>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<Patient, bool>>>(), Arg.Any<CancellationToken>(), Domain.Abstractions.FetchMode.Include)
				.Returns(Result.Failure<List<Patient>>(PersistenceErrors.IncorrectCount));

			//Act
			var result = await _handler.Handle(new PatientGetAllQuery(PatientQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.IsFailure.Should().BeTrue();
		}
	}
}