using Application.Entity.Doctors.Queries;
using Application.Entity.Doctors.Queries.Get;
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

namespace Application.UnitTests.Entities.Doctors.Queries
{
	public class DoctorGetQueryTests : TestsTheoryData
	{
		private readonly DoctorGetQueryHandler _handler;
		private readonly IDoctorRepository _repository;
		private readonly Doctor _doctor;
		private readonly DoctorFilter _filter;

		public DoctorGetQueryTests()
		{
			_repository = Substitute.For<IDoctorRepository>();
			_doctor = Doctor.Create(Username.Create("Fullname").Value).Value;
			_filter = new DoctorFilter();
			_handler = new DoctorGetQueryHandler(_repository);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<Doctor, bool>>>(expr =>
				expr.Compile()(_doctor) == true), Arg.Any<CancellationToken>())
				.Returns(_doctor);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<Doctor, bool>>>(expr =>
				expr.Compile()(_doctor) == false), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<Doctor>(PersistenceErrors.Entity<Doctor>.NotFound));
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new DoctorGetQuery(DoctorQueries.GetById(_doctor.Id)), default);

			//Assert
			result.Value.Id.Should().Be(_doctor.Id);
			result.Value.Name.Should().Be(_doctor.Fullname.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new DoctorGetQuery(DoctorQueries.GetById(Guid.Parse(id))), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<Doctor>.NotFound);
		}

		[Theory]
		[MemberData(nameof(ValidNameGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
		{
			//Act
			_filter.Name = name;
			var result = await _handler.Handle(new DoctorGetQuery(DoctorQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Id.Should().Be(_doctor.Id);
			result.Value.Name.Should().Be(_doctor.Fullname.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidNameGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
		{
			//Act
			_filter.Name = name;
			var result = await _handler.Handle(new DoctorGetQuery(DoctorQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<Doctor>.NotFound);
		}
		[Theory]
		[MemberData(nameof(ValidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new DoctorGetQuery(DoctorQueries.GetByFilter(_filter)), default);

			//Assert
			result.Value.Id.Should().Be(_doctor.Id);
			result.Value.Name.Should().Be(_doctor.Fullname.Value);
		}

		[Theory]
		[MemberData(nameof(InvalidCreationDatesGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
		{
			//Act
			_filter.CreationDateFrom = startDate;
			_filter.CreationDateTo = endDate;
			var result = await _handler.Handle(new DoctorGetQuery(DoctorQueries.GetByFilter(_filter)), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<Doctor>.NotFound);
		}
	}
}