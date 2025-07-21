using Application.Entity.Procedures.Queries;
using Application.Entity.Procedures.Queries.GetAll;
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

namespace Application.UnitTests.Entities.Procedures.Queries
{
    public class ProcedureGetAllQueryTests : TestsTheoryData
    {
        private readonly ProcedureGetAllQueryHandler _handler;
        private readonly IProcedureRepository _repository;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly ProcedureType _procedureType1;
        private readonly ProcedureType _procedureType2;
        private readonly Procedure _procedure1;
        private readonly Procedure _procedure2;
        private readonly ProcedureFilter _filter;

        public ProcedureGetAllQueryTests()
        {
            _repository = Substitute.For<IProcedureRepository>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _procedureType1 = ProcedureType.Create(Title.Create("Title1"), "", 0).Value;
            _procedureType2 = ProcedureType.Create(Title.Create("Title2"), "", 0).Value;
            _procedure1 = Procedure.Create(_patientCard, _procedureType1, _procedureType1.StandartDuration).Value;
            _procedure2 = Procedure.Create(_patientCard, _procedureType2, _procedureType2.StandartDuration).Value;
            _filter = new ProcedureFilter();
            _handler = new ProcedureGetAllQueryHandler(_repository);

            _repository.GetAllAsync(Arg.Is<Expression<Func<Procedure, bool>>>(expr =>
                expr.Compile()(_procedure1) == true && expr.Compile()(_procedure2) == true), Arg.Any<CancellationToken>())
                .Returns(new List<Procedure>() { _procedure1, _procedure2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<Procedure, bool>>>(expr =>
                expr.Compile()(_procedure1) == true && expr.Compile()(_procedure2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Procedure>() { _procedure1 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<Procedure, bool>>>(expr =>
                expr.Compile()(_procedure2) == true && expr.Compile()(_procedure1) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Procedure>() { _procedure2 });

            _repository.GetAllAsync(Arg.Is<Expression<Func<Procedure, bool>>>(expr =>
                expr.Compile()(_procedure1) == false && expr.Compile()(_procedure2) == false), Arg.Any<CancellationToken>())
                .Returns(new List<Procedure>() { });
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new ProcedureGetAllQuery(ProcedureQueries.GetWithoutPredicate()), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_procedureType1.Id);
            result.Value[1].TypeId.Should().Be(_procedureType2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_procedureType1.Id);
            result.Value[1].TypeId.Should().Be(_procedureType2.Id);
        }

        [Theory]
        [MemberData(nameof(ValidOnlyOneTitleGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidOnlyOneTitle(string name)
        {
            //Act
            _filter.Typename = name;
            var result = await _handler.Handle(new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(_filter)), default);

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
            var result = await _handler.Handle(new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value[0].TypeId.Should().Be(_procedureType1.Id);
            result.Value[1].TypeId.Should().Be(_procedureType2.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.Count.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(ValidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Expression<Func<Procedure, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(new List<Procedure>() { _procedure1, _procedure2 }.Skip(startIndex).Take(count).ToList());

            //Act
            var result = await _handler.Handle(new ProcedureGetAllQuery(ProcedureQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.Value.Count.Should().Be(count - startIndex);
        }
        [Theory]
        [MemberData(nameof(InvalidIndexesGetAllTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidIndexes(int startIndex, int count)
        {
            //Arrange
            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<Procedure, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<Procedure>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Is<int>(x => x < 0), Arg.Any<int>(), Arg.Any<Expression<Func<Procedure, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<Procedure>>(PersistenceErrors.IncorrectStartIndex));

            _repository.GetAllAsync(Arg.Any<int>(), Arg.Is<int>(x => x < 1), Arg.Any<Expression<Func<Procedure, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<List<Procedure>>(PersistenceErrors.IncorrectCount));

            //Act
            var result = await _handler.Handle(new ProcedureGetAllQuery(ProcedureQueries.GetWithoutPredicate(), startIndex, count), default);

            //Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}