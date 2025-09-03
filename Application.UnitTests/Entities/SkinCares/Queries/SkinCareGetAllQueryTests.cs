using Application.Entity.SkinCares.Queries;
using Application.Entity.SkinCares.Queries.GetAll;
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

namespace Application.UnitTests.Entities.SkinCares.Queries
{
	public class SkinCareGetAllQueryTests : TestsTheoryData
	{
		private readonly SkinCareGetAllQueryHandler _handler;
		private readonly ISkinCareRepository _repository;
		private readonly Patient _patient;
		private readonly PatientCard _patientCard;
		private readonly SkinCareType _skinCareType1;
		private readonly SkinCareType _skinCareType2;
		private readonly SkinCare _skincare1;
		private readonly SkinCare _skincare2;
		private readonly SkinCareFilter _filter;

		public SkinCareGetAllQueryTests()
		{
			_repository = Substitute.For<ISkinCareRepository>();
			_patient = Patient.Create(Username.Create("Fullname")).Value;
			_patientCard = _patient.Card;
			_skinCareType1 = SkinCareType.Create(Title.Create("Title1")).Value;
			_skinCareType2 = SkinCareType.Create(Title.Create("Title2")).Value;
			_skincare1 = SkinCare.Create(_patientCard, _skinCareType1).Value;
			_skincare2 = SkinCare.Create(_patientCard, _skinCareType2).Value;
			_filter = new SkinCareFilter();
			_handler = new SkinCareGetAllQueryHandler(_repository);

			_repository.GetAllAsync(Arg.Is<Expression<Func<SkinCare, bool>>>(expr =>
				expr.Compile()(_skincare1) == true && expr.Compile()(_skincare2) == true), Arg.Any<CancellationToken>())
				.Returns(new List<SkinCare>() { _skincare1, _skincare2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<SkinCare, bool>>>(expr =>
				expr.Compile()(_skincare1) == true && expr.Compile()(_skincare2) == false), Arg.Any<CancellationToken>())
				.Returns(new List<SkinCare>() { _skincare1 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<SkinCare, bool>>>(expr =>
				expr.Compile()(_skincare2) == true && expr.Compile()(_skincare1) == false), Arg.Any<CancellationToken>())
				.Returns(new List<SkinCare>() { _skincare2 });

			_repository.GetAllAsync(Arg.Is<Expression<Func<SkinCare, bool>>>(expr =>
				expr.Compile()(_skincare1) == false && expr.Compile()(_skincare2) == false), Arg.Any<CancellationToken>())
				.Returns(new List<SkinCare>() { });
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new SkinCareGetAllQuery(SkinCareQueries.GetWithoutPredicate()), default);

			//Assert
			result.Value[0].TypeId.Should().Be(_skinCareType1.Id);
			result.Value[1].TypeId.Should().Be(_skinCareType2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new SkinCareGetAllQuery(SkinCareQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].TypeId.Should().Be(_skinCareType1.Id);
			result.Value[1].TypeId.Should().Be(_skinCareType2.Id);
		}

		[Theory]
		[MemberData(nameof(ValidOnlyOneTitleGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneTitle(string name)
		{
			//Act
			_filter.Typename = name;
			var result = await _handler.Handle(new SkinCareGetAllQuery(SkinCareQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(1);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
		{
			//Act
			_filter.PatientName = name;
			var result = await _handler.Handle(new SkinCareGetAllQuery(SkinCareQueries.GetByFilter(_filter)), default);

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
			var result = await _handler.Handle(new SkinCareGetAllQuery(SkinCareQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value[0].TypeId.Should().Be(_skinCareType1.Id);
			result.Value[1].TypeId.Should().Be(_skinCareType2.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new SkinCareGetAllQuery(SkinCareQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Count.Should().Be(0);
		}
		[Theory]
		[MemberData(nameof(InvalidIndexesGetAllTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
		{
			//Arrange
			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<SkinCare, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<SkinCare>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<SkinCare, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<SkinCare>>(PersistenceErrors.IncorrectStartIndex));

			_repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<SkinCare, bool>>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<List<SkinCare>>(PersistenceErrors.IncorrectCount));

			//Act
			var result = await _handler.Handle(new SkinCareGetAllQuery(SkinCareQueries.GetWithoutPredicate(), startIndex, count), default);

			//Assert
			result.IsFailure.Should().BeTrue();
		}
	}
}