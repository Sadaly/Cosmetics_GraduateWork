//using Application.Entity.AgeChanges.Commands.SoftDelete;
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
//    public class AgeChangeSoftDeleteCommandTests : TestsTheoryData
//    {
//        private readonly AgeChangeSoftDeleteCommandHandler _handler;
//        private readonly IAgeChangeRepository _repository;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly AgeChange agechange = AgeChange.Create(Username.Create("Fullname")).Value;

//        public AgeChangeSoftDeleteCommandTests()
//        {
//            _repository = Substitute.For<IAgeChangeRepository>();
//            _unitOfWork = Substitute.For<IUnitOfWork>();

//            _handler = new AgeChangeSoftDeleteCommandHandler(_repository, _unitOfWork);

//            _repository.RemoveAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
//                .Returns(c => c.Arg<Result<AgeChange>>());

//            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == agechange.Id), Arg.Any<CancellationToken>())
//                .Returns(agechange);

//            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != agechange.Id), Arg.Any<CancellationToken>())
//                .Returns(Result.Failure<AgeChange>(PersistenceErrors.Entity<AgeChange>.IsSoftDeleted));

//            _unitOfWork.SaveChangesAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
//                .Returns(c => c.Arg<Result<AgeChange>>());
//        }

//        [Fact]
//        public async Task Handle_Should_ReturnSuccess_WhenValidId()
//        {
//            //Act
//            var result = await _handler.Handle(new AgeChangeSoftDeleteCommand(agechange.Id), default);

//            //Assert
//            result.Value.Should().Be(agechange.Id);
//        }

//        [Theory]
//        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
//        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
//        {
//            //Act
//            var result = await _handler.Handle(new AgeChangeSoftDeleteCommand(Guid.Parse(id)), default);

//            result.Error.Should().Be(PersistenceErrors.Entity<AgeChange>.IsSoftDeleted);
//        }
//    }
//}