using Application.Entity.ExternalProcedureRecords.Queries;
using Application.Entity.ExternalProcedureRecords.Queries.Get;
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

namespace Application.UnitTests.Entities.ExternalProcedureRecords.Queries
{
	public class ExternalProcedureRecordGetQueryTests : TestsTheoryData
	{
		private readonly ExternalProcedureRecordGetQueryHandler _handler;
		private readonly IExternalProcedureRecordRepository _repository;
		private readonly Patient _patient;
		private readonly PatientCard _patientCard;
		private readonly ExternalProcedureRecordType _externalProcedureRecordType;
		private readonly ExternalProcedureRecord _externalprocedurerecord;
		private readonly ExternalProcedureRecordFilter _filter;

		public ExternalProcedureRecordGetQueryTests()
		{
			_repository = Substitute.For<IExternalProcedureRecordRepository>();
			_patient = Patient.Create(Username.Create("Fullname")).Value;
			_patientCard = _patient.Card;
			_externalProcedureRecordType = ExternalProcedureRecordType.Create(Title.Create("Title1")).Value;
			_externalprocedurerecord = ExternalProcedureRecord.Create(_patientCard, _externalProcedureRecordType).Value;
			_filter = new ExternalProcedureRecordFilter();
			_handler = new ExternalProcedureRecordGetQueryHandler(_repository);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<ExternalProcedureRecord, bool>>>(expr =>
				expr.Compile()(_externalprocedurerecord) == true), Arg.Any<CancellationToken>())
				.Returns(_externalprocedurerecord);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<ExternalProcedureRecord, bool>>>(expr =>
				expr.Compile()(_externalprocedurerecord) == false), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<ExternalProcedureRecord>(PersistenceErrors.Entity<ExternalProcedureRecord>.NotFound));
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new ExternalProcedureRecordGetQuery(ExternalProcedureRecordQueries.GetById(_externalprocedurerecord.Id)), default);

			//Assert
			result.Value.TypeId.Should().Be(_externalProcedureRecordType.Id);
			result.Value.PatientCardId.Should().Be(_patientCard.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new ExternalProcedureRecordGetQuery(ExternalProcedureRecordQueries.GetById(Guid.Parse(id))), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<ExternalProcedureRecord>.NotFound);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new ExternalProcedureRecordGetQuery(ExternalProcedureRecordQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.TypeId.Should().Be(_externalProcedureRecordType.Id);
			result.Value.PatientCardId.Should().Be(_patientCard.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new ExternalProcedureRecordGetQuery(ExternalProcedureRecordQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<ExternalProcedureRecord>.NotFound);
		}
		[Theory]
		[MemberData(nameof(ValidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new ExternalProcedureRecordGetQuery(ExternalProcedureRecordQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.TypeId.Should().Be(_externalProcedureRecordType.Id);
			result.Value.PatientCardId.Should().Be(_patientCard.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new ExternalProcedureRecordGetQuery(ExternalProcedureRecordQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<ExternalProcedureRecord>.NotFound);
		}
	}
}