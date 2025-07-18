//using Application.Entity.Users.Commands.Create;
//using Application.UnitTests.TheoryData;
//using Domain.Abstractions;
//using Domain.Entity;
//using Domain.Repositories;
//using Domain.Shared;
//using FluentAssertions;
//using NSubstitute;

//namespace Application.UnitTests.Entities.Users.Commands
//{
//    public class UserCreateCommandTests : TestsTheoryData
//    {
//        private readonly UserCreateCommandHandler _handler;
//        private readonly IUserRepository _repository;
//        private readonly IUnitOfWork _unitOfWork;

//        public UserCreateCommandTests()
//        {
//            _repository = Substitute.For<IUserRepository>();
//            _unitOfWork = Substitute.For<IUnitOfWork>();

//            _handler = new UserCreateCommandHandler(_repository, _unitOfWork);

//            _repository.AddAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
//                .Returns(c => c.Arg<Result<User>>());

//            _unitOfWork.SaveChangesAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
//                .Returns(c => c.Arg<Result<User>>());
//        }

//        [Theory]
//        [MemberData(nameof(InvalidNameCreationTestCases))]
//        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
//        {
//            //Act
//            var result = await _handler.Handle(new UserCreateCommand(Name), default);

//            //Assert
//            result.Error.Code.Should().Be(expectedErrorCode);
//        }

//        [Theory]
//        [MemberData(nameof(ValidNameCreationTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
//        {
//            //Act
//            var result = await _handler.Handle(new UserCreateCommand(Name), default);

//            //Assert
//            result.IsSuccess.Should().Be(true);
//        }
//    }
//}