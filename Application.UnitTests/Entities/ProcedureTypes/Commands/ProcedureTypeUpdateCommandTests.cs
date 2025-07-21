using Application.Entity.ProcedureTypes.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.ProcedureTypes.Commands
{
    public class ProcedureTypeUpdateCommandTests : TestsTheoryData
    {
        private readonly ProcedureTypeUpdateCommandHandler _handler;
        private readonly IProcedureTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProcedureType _proceduretype;

        public ProcedureTypeUpdateCommandTests()
        {
            _repository = Substitute.For<IProcedureTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _proceduretype = ProcedureType.Create(Title.Create("Fullname").Value, "", 0).Value;

            _handler = new ProcedureTypeUpdateCommandHandler(_repository, _unitOfWork);
            _repository.UpdateAsync(Arg.Any<Result<ProcedureType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ProcedureType>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _proceduretype.Id), Arg.Any<CancellationToken>())
                .Returns(_proceduretype);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _proceduretype.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ProcedureType>(PersistenceErrors.Entity<ProcedureType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<ProcedureType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ProcedureType>>());
        }

        [Theory]
        [MemberData(nameof(InvalidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new ProcedureTypeUpdateCommand(_proceduretype.Id, Name), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
        {
            //Act
            var result = await _handler.Handle(new ProcedureTypeUpdateCommand(_proceduretype.Id, Name), default);

            //Assert
            result.Value.Should().Be(_proceduretype.Id);
            _proceduretype.Title.Should().Be(Title.Create(Name).Value);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new ProcedureTypeUpdateCommand(Guid.Parse(id), _proceduretype.Title.Value), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<ProcedureType>.NotFound);
        }
        [Fact]
        public async Task Handle_Should_ReturnError_WhenSaveIsFailer()
        {
            //Arrange
            _unitOfWork.SaveChangesAsync(Arg.Any<Result<ProcedureType>>(), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ProcedureType>(PersistenceErrors.Entity<ProcedureType>.NotFound));

            //Act
            var result = await _handler.Handle(new ProcedureTypeUpdateCommand(_proceduretype.Id, _proceduretype.Title.Value), default);

            //Assert
            result.IsFailure.Should().Be(true);
        }
    }
}