using Application.Entity.Doctors.Queries;
using Application.Entity.Doctors.Queries.GetAll;
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
    public class DoctorGetAllQueryTests : TestsTheoryData
    {
        private readonly DoctorGetAllQueryHandler _handler;
        private readonly IDoctorRepository _repository;
        private readonly Doctor _doctor1;
        private readonly Doctor _doctor2;
        private readonly DoctorFilter _filter;

        public DoctorGetAllQueryTests()
        {
            _repository = Substitute.For<IDoctorRepository>();
            _doctor1 = Doctor.Create(Username.Create("Fullname1").Value).Value;
            _doctor2 = Doctor.Create(Username.Create("Fullname2").Value).Value;
            _filter = new DoctorFilter();
            _handler = new DoctorGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<Doctor, bool>>>(expr =>
                expr.Compile()(_doctor1) == true && expr.Compile()(_doctor2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<Doctor>(){ _doctor1, _doctor2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<Doctor, bool>>>(expr =>
                expr.Compile()(_doctor1) == true && expr.Compile()(_doctor2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Doctor>(){ _doctor1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<Doctor, bool>>>(expr =>
                expr.Compile()(_doctor2) == true && expr.Compile()(_doctor1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Doctor>(){ _doctor2 });
            
            _repository.GetAllAsync(Arg.Is<Expression<Func<Doctor, bool>>>(expr =>
                expr.Compile()(_doctor1) == false && expr.Compile()(_doctor2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Doctor>(){});
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].Id.Should().Be(_doctor1.Id);
            result.Value[1].Id.Should().Be(_doctor2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.Name = name;
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_doctor1.Id);
            result.Value[1].Id.Should().Be(_doctor2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
        {
            //Act
            _filter.Name = name;
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidName(string name)
        {
            //Act
            _filter.Name = name;
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetByFilter(_filter)), default);

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
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(_doctor1.Id);
            result.Value[1].Id.Should().Be(_doctor2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnNone_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(ValidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<Doctor, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Doctor>() { _doctor1, _doctor2 }.Skip(startIndex).Take(count).ToList());
            
            //Act
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.Value.Count.Should().Be(count - startIndex);
        }
        [Theory]
        [MemberData(nameof(InvalidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<Doctor, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<Doctor>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<Doctor, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<Doctor>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<Doctor, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<Doctor>>(PersistenceErrors.IncorrectCount));

            //Act
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}