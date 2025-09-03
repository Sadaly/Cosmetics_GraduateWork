using Application.Entity.AgeChanges.Queries;
using Application.Entity.AgeChanges.Queries.Get;
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

namespace Application.UnitTests.Entities.AgeChanges.Queries
{
	public class AgeChangeGetQueryTests : TestsTheoryData
	{
		private readonly AgeChangeGetQueryHandler _handler;
		private readonly IAgeChangeRepository _repository;
		private readonly Patient _patient;
		private readonly PatientCard _patientCard;
		private readonly AgeChangeType _ageChangeType;
		private readonly AgeChange _agechange;
		private readonly AgeChangeFilter _filter;

		public AgeChangeGetQueryTests()
		{
			_repository = Substitute.For<IAgeChangeRepository>();
			_patient = Patient.Create(Username.Create("Fullname")).Value;
			_patientCard = _patient.Card;
			_ageChangeType = AgeChangeType.Create(Title.Create("Title1")).Value;
			_agechange = AgeChange.Create(_patientCard, _ageChangeType).Value;
			_filter = new AgeChangeFilter();
			_handler = new AgeChangeGetQueryHandler(_repository);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
				expr.Compile()(_agechange) == true), Arg.Any<CancellationToken>())
				.Returns(_agechange);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<AgeChange, bool>>>(expr =>
				expr.Compile()(_agechange) == false), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<AgeChange>(PersistenceErrors.Entity<AgeChange>.NotFound));
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetById(_agechange.Id)), default);

			//Assert
			result.Value.TypeId.Should().Be(_ageChangeType.Id);
			result.Value.PatientCardId.Should().Be(_patientCard.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetById(Guid.Parse(id))), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<AgeChange>.NotFound);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.TypeId.Should().Be(_ageChangeType.Id);
			result.Value.PatientCardId.Should().Be(_patientCard.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<AgeChange>.NotFound);
		}
		[Theory]
		[MemberData(nameof(ValidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.TypeId.Should().Be(_ageChangeType.Id);
			result.Value.PatientCardId.Should().Be(_patientCard.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new AgeChangeGetQuery(AgeChangeQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<AgeChange>.NotFound);
		}
	}
}