using Application.Entity.Patients.Commands.Create;
using Application.Entity.Patients.Commands.SoftDelete;
using Application.Entity.Patients.Commands.Update;
using Application.Entity.Patients.Queries;
using Application.Entity.Patients.Queries.Get;
using Application.Entity.Patients.Queries.GetAll;
using Domain.SupportData.Filters;

namespace Application.IntegrationTests.Tests
{
    public class PatientTests : BaseIntegrationTest
    {
        private readonly static string _name = "Fullname";

        private readonly PatientCreateCommand create;
        private readonly PatientCreateCommand create1;
        private readonly PatientCreateCommand create2;
        public PatientTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            create = new(_name);
            create1 = new(_name + "1");
            create2 = new(_name + "1");
        }

        [Fact]
        public async Task Create_ShouldAddPatient_WhenCorrectInput()
        {
            //Arrange

            //Act
            var id = await Sender.Send(create);

            //Assert
            Assert.True(dbContext.Patients.FirstOrDefault(d => d.Id == id.Value) != null);
        }

        [Fact]
        public async Task Create_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var command = new PatientCreateCommand("");

            //Act
            var id = await Sender.Send(command);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task Update_ShouldUpdatePatient_WhenCorrectInput()
        {
            //Arrange
            var id = await Sender.Send(create);

            //Act
            var newName = _name + "1234";
            var update = new PatientUpdateCommand(id.Value, newName);
            id = await Sender.Send(update);

            //Assert
            Assert.True(dbContext.Patients.FirstOrDefault(d => d.Id == id.Value && d.Fullname.Value == newName) != null);
        }

        [Fact]
        public async Task Update_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var id = await Sender.Send(create);

            //Act
            var update = new PatientUpdateCommand(id.Value, "");
            id = await Sender.Send(update);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldRemovePatient_WhenExists()
        {
            //Arrange
            var create = new PatientCreateCommand(_name);
            var id = await Sender.Send(create);
            var command = new PatientSoftDeleteCommand(id.Value);

            //Act
            var remove = await Sender.Send(command);
            var query = new PatientGetQuery(PatientQueries.GetById(id.Value));
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var command = new PatientSoftDeleteCommand(Guid.NewGuid());

            //Act
            var result = await Sender.Send(command);

            //Assert
            Assert.True(result.IsFailure);
        }
        [Fact]
        public async Task GetById_ShouldReturnPatient_WhenExists()
        {
            //Arrange
            var id = await Sender.Send(create);
            var query = new PatientGetQuery(PatientQueries.GetById(id.Value));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.PatientId == id.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var query = new PatientGetQuery(PatientQueries.GetById(Guid.NewGuid()));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Theory]
        [MemberData(nameof(CorrectFilterResults))]
        public async Task GetAllByFilter_ShouldReturnPatient_WhenExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
        {
            //Arrange
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new PatientFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                Fullname = name,
            };
            var query = new PatientGetAllQuery(PatientQueries.GetByFilter(filter));

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

            var filter = new PatientFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                Fullname = name,
            };
            var query = new PatientGetAllQuery(PatientQueries.GetByFilter(filter));

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
        public async Task Take_ShouldReturnPatient_WhenCorrectIndexes(int startIndex, int count)
        {
            //Arrange
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new PatientFilter();

            var query = new PatientGetAllQuery(PatientQueries.GetByFilter(filter), startIndex, count);

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

            var filter = new PatientFilter();
            var query = new PatientGetAllQuery(PatientQueries.GetByFilter(filter), startIndex, count);

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