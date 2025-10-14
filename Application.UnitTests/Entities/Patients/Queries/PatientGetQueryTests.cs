using Application.Entity.Patients.Queries;
using Application.Entity.Patients.Queries.Get;
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
	public class PatientGetQueryTests : TestsTheoryData
	{
		private readonly PatientGetQueryHandler _handler;
		private readonly IPatientRepository _repository;
		private readonly Patient _patient;
		private readonly PatientFilter _filter;

		public PatientGetQueryTests()
		{
			_repository = Substitute.For<IPatientRepository>();
			_patient = Patient.Create(Username.Create("Fullname").Value).Value;
			_filter = new PatientFilter();
			_handler = new PatientGetQueryHandler(_repository);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<Patient, bool>>>(expr =>
				expr.Compile()(_patient) == true), Arg.Any<CancellationToken>(), Domain.Abstractions.FetchMode.Include)
				.Returns(_patient);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<Patient, bool>>>(expr =>
				expr.Compile()(_patient) == false), Arg.Any<CancellationToken>(), Domain.Abstractions.FetchMode.Include)
				.Returns(Result.Failure<Patient>(PersistenceErrors.Entity<Patient>.NotFound));
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new PatientGetQuery(PatientQueries.GetById(_patient.Id)), default);

			//Assert
			result.Value.PatientId.Should().Be(_patient.Id);
			result.Value.Fullname.Should().Be(_patient.Fullname.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new PatientGetQuery(PatientQueries.GetById(Guid.Parse(id))), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<Patient>.NotFound);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.Fullname = name;
			var result = await _handler.Handle(new PatientGetQuery(PatientQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.PatientId.Should().Be(_patient.Id);
			result.Value.Fullname.Should().Be(_patient.Fullname.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
		{
			//Act
			_filter.Fullname = name;
			var result = await _handler.Handle(new PatientGetQuery(PatientQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<Patient>.NotFound);
		}
		[Theory]
		[MemberData(nameof(ValidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new PatientGetQuery(PatientQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.PatientId.Should().Be(_patient.Id);
			result.Value.Fullname.Should().Be(_patient.Fullname.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new PatientGetQuery(PatientQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<Patient>.NotFound);
		}
	}
}