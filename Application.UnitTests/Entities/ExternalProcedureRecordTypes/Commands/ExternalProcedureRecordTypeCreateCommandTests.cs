using Application.Entity.ExternalProcedureRecordTypes.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Repositories;
using Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.ExternalProcedureRecordTypes.Commands
{
    public class ExternalProcedureRecordTypeCreateCommandTests : TestsTheoryData
    {
        private readonly ExternalProcedureRecordTypeCreateCommandHandler _handler;
        private readonly IExternalProcedureRecordTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ExternalProcedureRecordTypeCreateCommandTests()
        {
            _repository = Substitute.For<IExternalProcedureRecordTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _handler = new ExternalProcedureRecordTypeCreateCommandHandler(_repository, _unitOfWork);

            _repository.AddAsync(Arg.Any<Result<ExternalProcedureRecordType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ExternalProcedureRecordType>>());

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<ExternalProcedureRecordType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<ExternalProcedureRecordType>>());
        }

        [Theory]
        [MemberData(nameof(InvalidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Title, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordTypeCreateCommand(Title), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidTitleInput(string Title)
        {
            //Act
            var result = await _handler.Handle(new ExternalProcedureRecordTypeCreateCommand(Title), default);

            //Assert
            result.IsSuccess.Should().Be(true);
        }
    }
}