using Application.Entity.HealthCondTypes.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.HealthCondTypes.Commands
{
    public class HealthCondTypeUpdateCommandTests : TestsTheoryData
    {
        private readonly HealthCondTypeUpdateCommandHandler _handler;
        private readonly IHealthCondTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HealthCondType _healthcondtype;

        public HealthCondTypeUpdateCommandTests()
        {
            _repository = Substitute.For<IHealthCondTypeRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _healthcondtype = HealthCondType.Create(Title.Create("Fullname").Value).Value;

            _handler = new HealthCondTypeUpdateCommandHandler(_repository, _unitOfWork);
            _repository.UpdateAsync(Arg.Any<Result<HealthCondType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<HealthCondType>>());

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _healthcondtype.Id), Arg.Any<CancellationToken>())
                .Returns(_healthcondtype);

            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _healthcondtype.Id), Arg.Any<CancellationToken>())
                .Returns(Result.Failure<HealthCondType>(PersistenceErrors.Entity<HealthCondType>.NotFound));

            _unitOfWork.SaveChangesAsync(Arg.Any<Result<HealthCondType>>(), Arg.Any<CancellationToken>())
                .Returns(c => c.Arg<Result<HealthCondType>>());
        }

        [Theory]
        [MemberData(nameof(InvalidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
        {
            //Act
            var result = await _handler.Handle(new HealthCondTypeUpdateCommand(_healthcondtype.Id, Name), default);

            //Assert
            result.Error.Code.Should().Be(expectedErrorCode);
        }

        [Theory]
        [MemberData(nameof(ValidTitleCreationTestCases))]
        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
        {
            //Act
            var result = await _handler.Handle(new HealthCondTypeUpdateCommand(_healthcondtype.Id, Name), default);

            //Assert
            result.Value.Should().Be(_healthcondtype.Id);
            _healthcondtype.Title.Should().Be(Title.Create(Name).Value);
        }

        [Theory]
        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
        {
            //Act
            var result = await _handler.Handle(new HealthCondTypeUpdateCommand(Guid.Parse(id), _healthcondtype.Title.Value), default);

            //Assert
            result.Error.Should().Be(PersistenceErrors.Entity<HealthCondType>.NotFound);
        }
    }
}