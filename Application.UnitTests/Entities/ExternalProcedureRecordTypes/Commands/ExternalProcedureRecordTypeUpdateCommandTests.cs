using Application.Entity.AgeChangeTypes.Commands.Update;
using Application.Entity.ExternalProcedureRecordTypes.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.ExternalProcedureRecordTypes.Commands
{
    public class ExternalProcedureRecordTypeUpdateCommandTests : TestsTheoryData
    {
        private readonly ExternalProcedureRecordTypeUpdateCommandHandler _handler;
        private readonly IExternalProcedureRecordTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ExternalProcedureRecordType _externalprocedurerecordtype;

        public ExternalProcedureRecordTypeUpdateCommandTests()
        {
            _repository = Substitute.For<IExternalProcedureRecordTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _externalprocedurerecordtype = ExternalProcedureRecordType.Create(Title.Create("Fullname").Value).Value;

            _handler = new ExternalProcedureRecordTypeUpdateCommandHandler(_repository, _unitOfWork);
            _repository.UpdateAsync(Arg.Any<Result<ExternalProcedureRecordType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ExternalProcedureRecordType>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _externalprocedurerecordtype.Id), Arg.Any<CancellationToken>())
                .Returns(_externalprocedurerecordtype);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _externalprocedurerecordtype.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ExternalProcedureRecordType>(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<ExternalProcedureRecordType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ExternalProcedureRecordType>>());
        }

        [Theory]
        [MemberData(nameof(InvalidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordTypeUpdateCommand(_externalprocedurerecordtype.Id, Name), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordTypeUpdateCommand(_externalprocedurerecordtype.Id, Name), default);

            //Assert
            result.Value.Should().Be(_externalprocedurerecordtype.Id);
            _externalprocedurerecordtype.Title.Should().Be(Title.Create(Name).Value);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordTypeUpdateCommand(Guid.Parse(id), _externalprocedurerecordtype.Title.Value), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound);
        }
        [Fact]
        public async Task Handle_Should_ReturnError_WhenSaveIsFailer()
        {
            //Arrange
            _unitOfWork.SaveChangesAsync(Arg.Any<Result<ExternalProcedureRecordType>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ExternalProcedureRecordType>(PersistenceErrors.Entity<ExternalProcedureRecordType>.NotFound));

            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordTypeUpdateCommand(_externalprocedurerecordtype.Id, _externalprocedurerecordtype.Title.Value), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }
    }
}