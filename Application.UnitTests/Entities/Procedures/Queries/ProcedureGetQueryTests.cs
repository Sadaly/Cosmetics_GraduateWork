using Application.Entity.Procedures.Queries;
using Application.Entity.Procedures.Queries.Get;
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
    public class ProcedureGetQueryTests : TestsTheoryData
    {
        private readonly ProcedureGetQueryHandler _handler;
        private readonly IProcedureRepository _repository;
        private readonly Patient _patient;
        private readonly PatientCard _patientCard;
        private readonly ProcedureType _procedureType;
        private readonly Procedure _procedure;
        private readonly ProcedureFilter _filter;

        public ProcedureGetQueryTests()
        {
            _repository = Substitute.For<IProcedureRepository>();
            _patient = Patient.Create(Username.Create("Fullname")).Value;
            _patientCard = _patient.Card;
            _procedureType = ProcedureType.Create(Title.Create("Title1"), "", 0).Value;
            _procedure = Procedure.Create(_patientCard, _procedureType, _procedureType.StandartDuration).Value;
            _filter = new ProcedureFilter();
            _handler = new ProcedureGetQueryHandler(_repository);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<Procedure, bool>>>(expr =>
                expr.Compile()(_procedure) == true), Arg.Any<CancellationToken>())
                .Returns(_procedure);

            _repository.GetByPredicateAsync(Arg.Is<Expression<Func<Procedure, bool>>>(expr =>
                expr.Compile()(_procedure) == false), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<Procedure>(PersistenceErrors.Entity<Procedure>.NotFound));
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new ProcedureGetQuery(ProcedureQueries.GetById(_procedure.Id)), default);

            //Assert
            result.Value.TypeId.Should().Be(_procedureType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new ProcedureGetQuery(ProcedureQueries.GetById(Guid.Parse(id))), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<Procedure>.NotFound);
        }

        [Theory]
        [MemberData(nameof(ValidNameGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new ProcedureGetQuery(ProcedureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.TypeId.Should().Be(_procedureType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidNameGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidName(string name)
        {
            //Act
            _filter.PatienName = name;
            var result = await _handler.Handle(new ProcedureGetQuery(ProcedureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<Procedure>.NotFound);
        }
        [Theory]
        [MemberData(nameof(ValidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ProcedureGetQuery(ProcedureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Value.TypeId.Should().Be(_procedureType.Id);
            result.Value.PatientCardId.Should().Be(_patientCard.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationDatesGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidCreationDates(DateTime? startDate, DateTime? endDate)
        {
            //Act
            _filter.CreationDateFrom = startDate;
            _filter.CreationDateTo = endDate;
            var result = await _handler.Handle(new ProcedureGetQuery(ProcedureQueries.GetByFilter(_filter)), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<Procedure>.NotFound);
        }
    }
}