using Application.Entity.Doctors.Queries;
using Application.Entity.Doctors.Queries.GetAll;
using Application.UnitTests.TheoryData;
using Domain.Entity;
using Domain.Repositories;
using Domain.SupportData.Filters;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace Application.UnitTests.Doctors.Queries
{
    public class DoctorGetAllQueryTests : TestsTheoryData
    {
        private readonly DoctorGetAllQueryHandler _handler;
        private readonly IDoctorRepository _repository;
        private readonly Doctor doctor1 = Doctor.Create(Username.Create("Fullname1").Value).Value;
        private readonly Doctor doctor2 = Doctor.Create(Username.Create("Fullname2").Value).Value;
        private readonly DoctorFilter filter = new DoctorFilter();

        public DoctorGetAllQueryTests()
        {
            _repository = Substitute.For<IDoctorRepository>();

            _handler = new DoctorGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<Doctor, bool>>>(expr =>
                expr.Compile()(doctor1) == true && expr.Compile()(doctor2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<Doctor>(){ doctor1, doctor2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<Doctor, bool>>>(expr =>
                expr.Compile()(doctor1) == true && expr.Compile()(doctor2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Doctor>(){ doctor1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<Doctor, bool>>>(expr =>
                expr.Compile()(doctor2) == true && expr.Compile()(doctor1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Doctor>(){ doctor2 });
            
            _repository.GetAllAsync(Arg.Is<Expression<Func<Doctor, bool>>>(expr =>
                expr.Compile()(doctor1) == false && expr.Compile()(doctor2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Doctor>(){});
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].Id.Should().Be(doctor1.Id);
            result.Value[1].Id.Should().Be(doctor2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            filter.Name = name;
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetByFilter(filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(doctor1.Id);
            result.Value[1].Id.Should().Be(doctor2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneName(string name)
        {
            //Act
            filter.Name = name;
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetByFilter(filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            filter.Name = name;
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetByFilter(filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            filter.CreationDateFrom = startDate;
            filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetByFilter(filter)), default);

            //Assert
            result.Value[0].Id.Should().Be(doctor1.Id);
            result.Value[1].Id.Should().Be(doctor2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            filter.CreationDateFrom = startDate;
            filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new DoctorGetAllQuery(DoctorQueries.GetByFilter(filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }
    }
}