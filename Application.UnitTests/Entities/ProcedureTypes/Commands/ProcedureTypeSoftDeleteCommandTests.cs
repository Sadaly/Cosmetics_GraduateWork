using Application.Entity.ProcedureTypes.Commands.SoftDelete;
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
    public class ProcedureTypeSoftDeleteCommandTests : TestsTheoryData
    {
        private readonly ProcedureTypeSoftDeleteCommandHandler _handler;
        private readonly IProcedureTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProcedureType _proceduretype;

        public ProcedureTypeSoftDeleteCommandTests()
        {
            _repository = Substitute.For<IProcedureTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _proceduretype = ProcedureType.Create(Title.Create("Fullname").Value, "", 0).Value;

            _handler = new ProcedureTypeSoftDeleteCommandHandler(_repository, _unitOfWork);

            _repository.RemoveAsync(Arg.Any<Result<ProcedureType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ProcedureType>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _proceduretype.Id), Arg.Any<CancellationToken>())
                .Returns(_proceduretype);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _proceduretype.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<ProcedureType>(PersistenceErrors.Entity<ProcedureType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<ProcedureType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ProcedureType>>());
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenValidId()
        {
            //Act
            var result = await _handler.Handle(new ProcedureTypeSoftDeleteCommand(_proceduretype.Id), default);

            //Assert
            result.Value.Should().Be(_proceduretype.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new ProcedureTypeSoftDeleteCommand(Guid.Parse(id)), default);

            result.Error.Should().Be(PersistenceErrors.Entity<ProcedureType>.NotFound);
        }
    }
}