//using Application.Entity.AgeChanges.Commands.Update;
//using Application.UnitTests.TheoryData;
//using Domain.Abstractions;
//using Domain.Entity;
//using Domain.Errors;
//using Domain.Repositories;
//using Domain.Shared;
//using Domain.ValueObjects;
//using FluentAssertions;
//using NSubstitute;

//namespace Application.UnitTests.AgeChanges.Commands
//{
//    public class AgeChangeUpdateCommandTests : TestsTheoryData
//    {
//        private readonly AgeChangeUpdateCommandHandler _handler;
//        private readonly IAgeChangeRepository _repository;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly AgeChange agechange = AgeChange.Create(Username.Create("Fullname").Value).Value;

//        public AgeChangeUpdateCommandTests()
//        {
//            _repository = Substitute.For<IAgeChangeRepository>();
//            _unitOfWork = Substitute.For<IUnitOfWork>();

//            _handler = new AgeChangeUpdateCommandHandler(_repository, _unitOfWork);

//            _repository.UpdateAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
//                .Returns(c => c.Arg<Result<AgeChange>>());

//            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == agechange.Id), Arg.Any<CancellationToken>())
//                .Returns(agechange);

//            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != agechange.Id), Arg.Any<CancellationToken>())
//                .Returns(Result.Failure<AgeChange>(PersistenceErrors.Entity<AgeChange>.IsSoftDeleted));

//            _unitOfWork.SaveChangesAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
//                .Returns(c => c.Arg<Result<AgeChange>>());
//        }

//        [Theory]
//        [MemberData(nameof(InvalidNameCreationTestCases))]
//        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
//        {
//            //Act
//            var result = await _handler.Handle(new AgeChangeUpdateCommand(agechange.Id, Name), default);

//            //Assert
//            result.Error.Code.Should().Be(expectedErrorCode);
//        }

//        [Theory]
//        [MemberData(nameof(ValidNameCreationTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
//        {
//            //Act
//            var result = await _handler.Handle(new AgeChangeUpdateCommand(agechange.Id, Name), default);

//            //Assert
//            result.Value.Should().Be(agechange.Id);
//            agechange.Fullname.Should().Be(Username.Create(Name).Value);
//        }

//        [Theory]
//        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
//        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
//        {
//            //Act
//            var result = await _handler.Handle(new AgeChangeUpdateCommand(Guid.Parse(id), agechange.Fullname.Value), default);

//            //Assert
//            result.Error.Should().Be(PersistenceErrors.Entity<AgeChange>.IsSoftDeleted);
//        }
//    }
//}