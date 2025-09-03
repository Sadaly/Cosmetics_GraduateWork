using Application.Entity.PatientSpecificses.Queries;
using Application.Entity.PatientSpecificses.Queries.Get;
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
	public class PatientSpecificsGetQueryTests : TestsTheoryData
	{
		private readonly PatientSpecificsGetQueryHandler _handler;
		private readonly IPatientSpecificsRepository _repository;
		private readonly PatientSpecifics _patientspecifics;
		private readonly PatientSpecificsFilter _filter;

		public PatientSpecificsGetQueryTests()
		{
			_repository = Substitute.For<IPatientSpecificsRepository>();
			_patientspecifics = PatientSpecifics.Create("", "", "", "", Patient.Create(Username.Create("Fullname")).Value.Card).Value;
			_filter = new PatientSpecificsFilter();
			_handler = new PatientSpecificsGetQueryHandler(_repository);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<PatientSpecifics, bool>>>(expr =>
				expr.Compile()(_patientspecifics) == true), Arg.Any<CancellationToken>())
				.Returns(_patientspecifics);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<PatientSpecifics, bool>>>(expr =>
				expr.Compile()(_patientspecifics) == false), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<PatientSpecifics>(PersistenceErrors.Entity<PatientSpecifics>.NotFound));
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new PatientSpecificsGetQuery(PatientSpecificsQueries.GetById(_patientspecifics.Id)), default);

			//Assert
			result.Value.Id.Should().Be(_patientspecifics.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new PatientSpecificsGetQuery(PatientSpecificsQueries.GetById(Guid.Parse(id))), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<PatientSpecifics>.NotFound);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new PatientSpecificsGetQuery(PatientSpecificsQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Id.Should().Be(_patientspecifics.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new PatientSpecificsGetQuery(PatientSpecificsQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<PatientSpecifics>.NotFound);
		}
		[Theory]
		[MemberData(nameof(ValidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new PatientSpecificsGetQuery(PatientSpecificsQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Id.Should().Be(_patientspecifics.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new PatientSpecificsGetQuery(PatientSpecificsQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<PatientSpecifics>.NotFound);
		}
	}
}