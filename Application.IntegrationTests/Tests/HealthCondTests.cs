using Application.Entity.HealthConds.Commands.ChangeType;
using Application.Entity.HealthConds.Commands.Create;
using Application.Entity.HealthConds.Commands.SoftDelete;
using Application.Entity.HealthConds.Queries;
using Application.Entity.HealthConds.Queries.Get;
using Application.Entity.HealthConds.Queries.GetAll;
using Application.Entity.HealthCondTypes.Commands.Create;
using Application.Entity.Patients.Commands.Create;
using Application.Entity.Patients.Queries;
using Application.Entity.Patients.Queries.Get;
using Domain.SupportData.Filters;

namespace Application.IntegrationTests.Tests
{
    public class HealthCondTests : BaseIntegrationTest
    {
        private readonly static string _typename = "Typename";
        private readonly static string _name = "Fullname";

        private readonly HealthCondTypeCreateCommand createType;
        private readonly HealthCondTypeCreateCommand createType1;
        private readonly HealthCondTypeCreateCommand createType2;

        private readonly PatientCreateCommand createPatient;
        public HealthCondTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            createType = new(_typename);
            createType1 = new(_typename + "1");
            createType2 = new(_typename + "1");

            createPatient = new(_name + "1");
        }

        [Fact]
        public async Task Create_ShouldAddHealthCond_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new HealthCondCreateCommand(patientCardId, typeId);

            //Act
            var id = await Sender.Send(create);

            //Assert
            Assert.True(dbContext.HealthConds.FirstOrDefault(d => d.Id == id.Value && d.TypeId == typeId) != null);
        }

        [Fact]
        public async Task Create_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var command = new HealthCondCreateCommand(Guid.NewGuid(), Guid.NewGuid());

            //Act
            var id = await Sender.Send(command);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task Update_ShouldChangeTypeHealthCond_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId1 = (await Sender.Send(createType1)).Value;
            var typeId2 = (await Sender.Send(createType2)).Value;
            var create = new HealthCondCreateCommand(patientCardId, typeId1);
            var id = await Sender.Send(create);

            //Act
            var update = new HealthCondChangeTypeCommand(id.Value, typeId2);
            id = await Sender.Send(update);

            //Assert
            Assert.True(dbContext.HealthConds.FirstOrDefault(d => d.Id == id.Value && d.TypeId == typeId2) != null);
        }

        [Fact]
        public async Task Update_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new HealthCondCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);

            //Act
            var update = new HealthCondChangeTypeCommand(id.Value, Guid.NewGuid());
            id = await Sender.Send(update);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldRemoveHealthCond_WhenExists()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new HealthCondCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);
            var command = new HealthCondSoftDeleteCommand(id.Value);

            //Act
            var remove = await Sender.Send(command);
            var query = new HealthCondGetQuery(HealthCondQueries.GetById(id.Value));
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var command = new HealthCondSoftDeleteCommand(Guid.NewGuid());

            //Act
            var result = await Sender.Send(command);

            //Assert
            Assert.True(result.IsFailure);
        }
        [Fact]
        public async Task GetById_ShouldReturnHealthCond_WhenExists()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new HealthCondCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);
            var query = new HealthCondGetQuery(HealthCondQueries.GetById(id.Value));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.TypeId == typeId);
        }

        [Fact]
        public async Task GetById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var query = new HealthCondGetQuery(HealthCondQueries.GetById(Guid.NewGuid()));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Theory]
        [MemberData(nameof(CorrectFilterResults))]
        public async Task GetAllByFilter_ShouldReturnHealthCond_WhenExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId1 = (await Sender.Send(createType1)).Value;
            var typeId2 = (await Sender.Send(createType2)).Value;
            var create1 = new HealthCondCreateCommand(patientCardId, typeId1);
            var create2 = new HealthCondCreateCommand(patientCardId, typeId2);
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new HealthCondFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                PatientName = name,
            };
            var query = new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(filter));

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
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId1 = (await Sender.Send(createType1)).Value;
            var typeId2 = (await Sender.Send(createType2)).Value;
            var create1 = new HealthCondCreateCommand(patientCardId, typeId1);
            var create2 = new HealthCondCreateCommand(patientCardId, typeId2);
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new HealthCondFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                PatientName = name,
            };
            var query = new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(filter));

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
        public async Task Take_ShouldReturnHealthCond_WhenCorrectIndexes(int startIndex, int count)
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId1 = (await Sender.Send(createType1)).Value;
            var typeId2 = (await Sender.Send(createType2)).Value;
            var create1 = new HealthCondCreateCommand(patientCardId, typeId1);
            var create2 = new HealthCondCreateCommand(patientCardId, typeId2);
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new HealthCondFilter();

            var query = new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(filter), startIndex, count);

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
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId1 = (await Sender.Send(createType1)).Value;
            var typeId2 = (await Sender.Send(createType2)).Value;
            var create1 = new HealthCondCreateCommand(patientCardId, typeId1);
            var create2 = new HealthCondCreateCommand(patientCardId, typeId2);
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new HealthCondFilter();
            var query = new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(filter), startIndex, count);

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