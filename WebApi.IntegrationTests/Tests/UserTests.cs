using Application.Entity.Users.Commands.Create;
using Application.Entity.Users.Commands.Login;
using Application.Entity.Users.Commands.SoftDelete;
using Application.Entity.Users.Commands.Update;
using Application.Entity.Users.Queries;
using Application.Entity.Users.Queries.Get;
using Application.Entity.Users.Queries.GetAll;
using Domain.SupportData.Filters;

namespace WebApi.IntegrationTests.Tests
{
    public class UserTests : BaseIntegrationTest
    {
        private static readonly string _name = "username";

        private readonly UserCreateCommand create;
        private readonly UserCreateCommand create1;
        private readonly UserCreateCommand create2;
        public UserTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            create = new(_name, "email@email", "password");
            create1 = new(_name + "1", "email1@email", "password");
            create2 = new(_name + "2", "email2@email", "password");
        }

        [Fact]
        public async Task Create_ShouldAddUser_WhenCorrectInput()
        {
            //Arrange

            //Act
            var id = await Sender.Send(create);

            //Assert
            Assert.True(dbContext.Users.FirstOrDefault(d => d.Id == id.Value) != null);
        }

        [Fact]
        public async Task Create_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var command = new UserCreateCommand("", "", "");

            //Act
            var id = await Sender.Send(command);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task Login_ShouldLoginUser_WhenCorrectInput()
        {
            //Arrange
            await Sender.Send(create);
            var command = new UserLoginCommand(create.Email, create.Password);

            //Act
            var token = await Sender.Send(command);

            //Assert
            Assert.True(token != null);
        }

        [Fact]
        public async Task Login_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var command = new UserLoginCommand("", "");

            //Act
            var token = await Sender.Send(command);

            //Assert
            Assert.True(token.IsFailure);
        }

        [Fact]
        public async Task Update_ShouldUpdateUser_WhenCorrectInput()
        {
            //Arrange
            var id = await Sender.Send(create);

            //Act
            var update = new UserUpdateCommand(id.Value, _name + "1", "email1@email", "password1");
            var token = (await Sender.Send(update)).Value;

            //Assert
            Assert.True(token != null);
            Assert.True(dbContext.Users.FirstOrDefault(d => d.Id == id.Value && d.Username.Value == update.Username) != null);
        }

        [Fact]
        public async Task Update_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var id = await Sender.Send(create);

            //Act
            var update = new UserUpdateCommand(id.Value, null, null, null);
            var token = await Sender.Send(update);

            //Assert
            Assert.True(token.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldRemoveUser_WhenExists()
        {
            //Arrange
            var id = await Sender.Send(create);
            var command = new UserSoftDeleteCommand(id.Value);

            //Act
            var remove = await Sender.Send(command);
            var query = new UserGetQuery(UserQueries.GetById(id.Value));
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var command = new UserSoftDeleteCommand(Guid.NewGuid());

            //Act
            var result = await Sender.Send(command);

            //Assert
            Assert.True(result.IsFailure);
        }
        [Fact]
        public async Task GetById_ShouldReturnUser_WhenExists()
        {
            //Arrange
            var id = await Sender.Send(create);
            var query = new UserGetQuery(UserQueries.GetById(id.Value));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.UserId == id.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var query = new UserGetQuery(UserQueries.GetById(Guid.NewGuid()));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Theory]
        [MemberData(nameof(CorrectFilterResults))]
        public async Task GetAllByFilter_ShouldReturnUser_WhenExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
        {
            //Arrange
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new UserFilter()
            {
                RegistrationDateFrom = creationDateFrom,
                RegistrationDateTo = creationDateTo,
                Username = name,
            };
            var query = new UserGetAllQuery(UserQueries.GetByFilter(filter));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.Count == 2);
        }

        [Theory]
        [MemberData(nameof(IncorrectFilterResults))]
        public async Task GetAllByFilter_ShouldReturnNone_WhenNotExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
        {
            //Arrange
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new UserFilter()
            {
                RegistrationDateFrom = creationDateFrom,
                RegistrationDateTo = creationDateTo,
                Username = name,
            };
            var query = new UserGetAllQuery(UserQueries.GetByFilter(filter));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.Count == 0);
        }
        public static TheoryData<string?, DateTime?, DateTime?> CorrectFilterResults = new()
        {
            { null, null, null },
            { null, null, DateTime.UtcNow.AddDays(1) },
            { null, DateTime.UtcNow.AddDays(-1), null },
            { null, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1) },
            { _name, null, null },
            { _name, null, DateTime.UtcNow.AddDays(1) },
            { _name, DateTime.UtcNow.AddDays(-1), null },
            { _name, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1) },
        };
        public static TheoryData<string?, DateTime?, DateTime?> IncorrectFilterResults = new()
        {
            { null, null, DateTime.UtcNow.AddDays(-1) },
            { null, DateTime.UtcNow.AddDays(1), null },
            { null, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(-1) },
            { _name + "3", null, null },
            { _name + "3", null, DateTime.UtcNow.AddDays(-1) },
            { _name + "3", DateTime.UtcNow.AddDays(1), null },
            { _name + "3", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(-1) },
        };



        [Theory]
        [MemberData(nameof(CorrectTakeDataResults))]
        public async Task Take_ShouldReturnUser_WhenCorrectIndexes(int startIndex, int count)
        {
            //Arrange
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new UserFilter();

            var query = new UserGetAllQuery(UserQueries.GetByFilter(filter), startIndex, count);

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.Count == 1);
        }

        [Theory]
        [MemberData(nameof(IncorrectTakeDataResults))]
        public async Task Take_ShouldReturnNone_WhenIncorrectIndexes(int startIndex, int count)
        {
            //Arrange
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new UserFilter();
            var query = new UserGetAllQuery(UserQueries.GetByFilter(filter), startIndex, count);

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure || result.Value.Count == 0);
        }
        public static TheoryData<int, int> CorrectTakeDataResults = new()
        {
            { 0, 1 },
            { 1, 1 },
        };
        public static TheoryData<int, int> IncorrectTakeDataResults = new()
        {
            { -1, 1 },
            { 1, 0 },
            { 2, 1 },
        };
    }
}