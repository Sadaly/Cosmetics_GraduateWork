//using Application.Entity.Users.Commands.Update;
//using Application.UnitTests.TheoryData;
//using Domain.Abstractions;
//using Domain.Entity;
//using Domain.Errors;
//using Domain.Repositories;
//using Domain.Shared;
//using Domain.ValueObjects;
//using FluentAssertions;
//using NSubstitute;

//namespace Application.UnitTests.Entities.Users.Commands
//{
//    public class UserUpdateCommandTests : TestsTheoryData
//    {
//        private readonly UserUpdateCommandHandler _handler;
//        private readonly IUserRepository _repository;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly User _user;

//        public UserUpdateCommandTests()
//        {
//            _repository = Substitute.For<IUserRepository>();
//            _unitOfWork = Substitute.For<IUnitOfWork>();
//            _user = User.Create(Username.Create("Fullname").Value).Value;

//            _handler = new UserUpdateCommandHandler(_repository, _unitOfWork);
//            _repository.UpdateAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
//                .Returns(c => c.Arg<Result<User>>());

//            _repository.GetByIdAsync(Arg.Is<Guid>(x => x == _user.Id), Arg.Any<CancellationToken>())
//                .Returns(_user);

//            _repository.GetByIdAsync(Arg.Is<Guid>(x => x != _user.Id), Arg.Any<CancellationToken>())
//                .Returns(Result.Failure<User>(PersistenceErrors.Entity<User>.NotFound));

//            _unitOfWork.SaveChangesAsync(Arg.Any<Result<User>>(), Arg.Any<CancellationToken>())
//                .Returns(c => c.Arg<Result<User>>());
//        }

//        [Theory]
//        [MemberData(nameof(InvalidNameCreationTestCases))]
//        public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
//        {
//            //Act
//            var result = await _handler.Handle(new UserUpdateCommand(_user.Id, Name), default);

//            //Assert
//            result.Error.Code.Should().Be(expectedErrorCode);
//        }

//        [Theory]
//        [MemberData(nameof(ValidNameCreationTestCases))]
//        public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
//        {
//            //Act
//            var result = await _handler.Handle(new UserUpdateCommand(_user.Id, Name), default);

//            //Assert
//            result.Value.Should().Be(_user.Id);
//            _user.Fullname.Should().Be(Username.Create(Name).Value);
//        }

//        [Theory]
//        [MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
//        public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
//        {
//            //Act
//            var result = await _handler.Handle(new UserUpdateCommand(Guid.Parse(id), _user.Fullname.Value), default);

//            //Assert
//            result.Error.Should().Be(PersistenceErrors.Entity<User>.NotFound);
//        }
//    }
//}