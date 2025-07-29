using Application.Entity.AgeChangeTypes.Commands.Create;
using Application.Entity.AgeChangeTypes.Commands.SoftDelete;
using Application.Entity.AgeChangeTypes.Commands.Update;
using Application.Entity.AgeChangeTypes.Queries;
using Application.Entity.AgeChangeTypes.Queries.Get;
using Application.Entity.AgeChangeTypes.Queries.GetAll;
using Domain.SupportData.Filters;
using Domain.ValueObjects;

namespace WebApi.IntegrationTests.Tests
{
    public class AgeChangeTypeTests : BaseIntegrationTest
    {
        private readonly static string _name = "Title";

        private readonly AgeChangeTypeCreateCommand create;
        private readonly AgeChangeTypeCreateCommand create1;
        private readonly AgeChangeTypeCreateCommand create2;
        public AgeChangeTypeTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            create = new(_name);
            create1 = new(_name + "1");
            create2 = new(_name + "1");
        }

        [Fact]
        public async Task Create_ShouldAddAgeChangeType_WhenCorrectInput()
        {
            //Arrange

            //Act
            var id = await Sender.Send(create);

            //Assert
            Assert.True(dbContext.AgeChangeTypes.FirstOrDefault(d => d.Id == id.Value) != null);
        }

        [Fact]
        public async Task Create_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var command = new AgeChangeTypeCreateCommand(new string('a', Title.MAX_LENGTH + 1));

            //Act
            var id = await Sender.Send(command);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task Update_ShouldUpdateAgeChangeType_WhenCorrectInput()
        {
            //Arrange
            var id = await Sender.Send(create);

            //Act
            var newName = _name + "1234";
            var update = new AgeChangeTypeUpdateCommand(id.Value, newName);
            id = await Sender.Send(update);

            //Assert
            Assert.True(dbContext.AgeChangeTypes.FirstOrDefault(d => d.Id == id.Value && d.Title.Value == newName) != null);
        }

        [Fact]
        public async Task Update_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var id = await Sender.Send(create);

            //Act
            var update = new AgeChangeTypeUpdateCommand(id.Value, new string('a', Title.MAX_LENGTH + 1));
            id = await Sender.Send(update);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldRemoveAgeChangeType_WhenExists()
        {
            //Arrange
            var create = new AgeChangeTypeCreateCommand(_name);
            var id = await Sender.Send(create);
            var command = new AgeChangeTypeSoftDeleteCommand(id.Value);

            //Act
            var remove = await Sender.Send(command);
            var query = new AgeChangeTypeGetQuery(AgeChangeTypeQueries.GetById(id.Value));
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var command = new AgeChangeTypeSoftDeleteCommand(Guid.NewGuid());

            //Act
            var result = await Sender.Send(command);

            //Assert
            Assert.True(result.IsFailure);
        }
        [Fact]
        public async Task GetById_ShouldReturnAgeChangeType_WhenExists()
        {
            //Arrange
            var id = await Sender.Send(create);
            var query = new AgeChangeTypeGetQuery(AgeChangeTypeQueries.GetById(id.Value));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.Id == id.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var query = new AgeChangeTypeGetQuery(AgeChangeTypeQueries.GetById(Guid.NewGuid()));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Theory]
        [MemberData(nameof(CorrectFilterResults))]
        public async Task GetAllByFilter_ShouldReturnAgeChangeType_WhenExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
        {
            //Arrange
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new AgeChangeTypeFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                Typename = name,
            };
            var query = new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetByFilter(filter));

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

            var filter = new AgeChangeTypeFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                Typename = name,
            };
            var query = new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetByFilter(filter));

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
        public async Task Take_ShouldReturnAgeChangeType_WhenCorrectIndexes(int startIndex, int count)
        {
            //Arrange
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new AgeChangeTypeFilter();

            var query = new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetByFilter(filter), startIndex, count);

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

            var filter = new AgeChangeTypeFilter();
            var query = new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetByFilter(filter), startIndex, count);

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