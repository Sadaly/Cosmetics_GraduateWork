//using Application.Entity.AgeChanges.Commands.Create;
//using Application.UnitTests.TheoryData;
//using Domain.Abstractions;
//using Domain.Entity;
//using Domain.Repositories;
//using Domain.Shared;
//using FluentAssertions;
//using NSubstitute;

//namespace Application.UnitTests.AgeChanges.Commands
//{
//    public class AgeChangeCreateCommandTests : TestsTheoryData
//    {
//        private readonly AgeChangeCreateCommandHandler _handler;
//        private readonly IAgeChangeRepository _repository;
//        private readonly IUnitOfWork _unitOfWork;

//        public AgeChangeCreateCommandTests()
//        {
//            _repository = Substitute.For<IAgeChangeRepository>();
//            _unitOfWork = Substitute.For<IUnitOfWork>();

//            _handler = new AgeChangeCreateCommandHandler(_repository, _unitOfWork);

//            _repository.AddAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
//                .Returns(c => c.Arg<Result<AgeChange>>());

//            _unitOfWork.SaveChangesAsync(Arg.Any<Result<AgeChange>>(), Arg.Any<CancellationToken>())
//                .Returns(c => c.Arg<Result<AgeChange>>());
//        }

//        [Theory]
//        [MemberData(nameof(InvalidNameCreationTestCases))]
//        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
//        {
//            //Act
//            var result = await _handler.Handle(new AgeChangeCreateCommand(Name), default);

//            //Assert
//            result.Error.Code.Should().Be(expectedErrorCode);
//        }

//        [Theory]
//        [MemberData(nameof(ValidNameCreationTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
//        {
//            //Act
//            var result = await _handler.Handle(new AgeChangeCreateCommand(Name), default);

//            //Assert
//            result.IsSuccess.Should().Be(true);
//        }
//    }
//}