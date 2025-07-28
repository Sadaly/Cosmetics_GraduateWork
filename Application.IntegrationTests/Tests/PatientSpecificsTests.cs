using Application.Entity.Patients.Commands.Create;
using Application.Entity.Patients.Queries;
using Application.Entity.Patients.Queries.Get;
using Domain.SupportData.Filters;
using Application.Entity.PatientSpecificses.Commands.Update;
using Application.Entity.PatientSpecificses.Queries.Get;
using Application.Entity.PatientSpecificses.Queries;
using Application.Entity.PatientSpecificses.Queries.GetAll;
using Application.Entity.PatientSpecificses.Commands.Create;

namespace Application.IntegrationTests.Tests
{
    public class PatientSpecificsTests : BaseIntegrationTest
    {
        private readonly static string _name = "Fullname";

        private readonly PatientCreateCommand createPatient;
        private readonly PatientCreateCommand createPatient1;
        private readonly PatientCreateCommand createPatient2;

        public PatientSpecificsTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            createPatient = new(_name);
            createPatient1 = new(_name + "1");
            createPatient2 = new(_name + "1");
        }

        [Fact]
        public async Task Create_ShouldAddPatientSpecifics_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;

            //Act
            var id = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId, "", "", "", ""))).Value;

            //Assert
            Assert.True(dbContext.PatientSpecificses.FirstOrDefault(d => d.Id == id) != null);
        }

        [Fact]
        public async Task Create_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var command = new PatientSpecificsCreateCommand(Guid.NewGuid(), "", "", "", "");

            //Act
            var id = await Sender.Send(command);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task Update_ShouldUpdatePatientSpecifics_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var id = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId, "", "", "", "")));

            //Act
            var update = new PatientSpecificsUpdateCommand(id.Value, "asdasd", "adsas", "sadads", "asdas");
            id = (await Sender.Send(update));

            //Assert
            Assert.True(dbContext.PatientSpecificses.FirstOrDefault(d => d.Id == id.Value && d.Sleep == update.Sleep) != null);
        }

        [Fact]
        public async Task Update_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            
            //Act
            var command = new PatientSpecificsUpdateCommand(Guid.NewGuid(), "", "", "", "asdasa2");
            var update = await Sender.Send(command);

            //Assert
            Assert.True(update.IsFailure);
        }

        [Fact]
        public async Task GetById_ShouldReturnPatientSpecifics_WhenExists()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var id = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId, "", "", "", ""))).Value;
            var query = new PatientSpecificsGetQuery(PatientSpecificsQueries.GetById(id));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.Id == id);
        }

        [Fact]
        public async Task GetById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var query = new PatientSpecificsGetQuery(PatientSpecificsQueries.GetById(Guid.NewGuid()));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Theory]
        [MemberData(nameof(CorrectFilterResults))]
        public async Task GetAllByFilter_ShouldReturnPatientSpecifics_WhenExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
        {
            //Arrange
            var patientId1 = (await Sender.Send(createPatient1)).Value;
            var patientCardId1 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId1)))).Value.CardtId;
            var id1 = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId1, "", "", "", ""))).Value;

            var patientId2 = (await Sender.Send(createPatient2)).Value;
            var patientCardId2 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId2)))).Value.CardtId;
            var id2 = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId2, "", "", "", ""))).Value;

            var filter = new PatientSpecificsFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                PatientName = name,
            };
            var query = new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(filter));

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
            var patientId1 = (await Sender.Send(createPatient1)).Value;
            var patientCardId1 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId1)))).Value.CardtId;
            var id1 = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId1, "", "", "", ""))).Value;

            var patientId2 = (await Sender.Send(createPatient2)).Value;
            var patientCardId2 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId2)))).Value.CardtId;
            var id2 = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId2, "", "", "", ""))).Value;


            var filter = new PatientSpecificsFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                PatientName = name,
            };
            var query = new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(filter));

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
        public async Task Take_ShouldReturnPatientSpecifics_WhenCorrectIndexes(int startIndex, int count)
        {
            //Arrange
            var patientId1 = (await Sender.Send(createPatient1)).Value;
            var patientCardId1 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId1)))).Value.CardtId;
            var id1 = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId1, "", "", "", ""))).Value;

            var patientId2 = (await Sender.Send(createPatient2)).Value;
            var patientCardId2 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId2)))).Value.CardtId;
            var id2 = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId2, "", "", "", ""))).Value;

            var filter = new PatientSpecificsFilter();

            var query = new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(filter), startIndex, count);

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
            var patientId1 = (await Sender.Send(createPatient1)).Value;
            var patientCardId1 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId1)))).Value.CardtId;
            var id1 = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId1, "", "", "", ""))).Value;

            var patientId2 = (await Sender.Send(createPatient2)).Value;
            var patientCardId2 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId2)))).Value.CardtId;
            var id2 = (await Sender.Send(new PatientSpecificsCreateCommand(patientCardId2, "", "", "", ""))).Value;

            var filter = new PatientSpecificsFilter();
            var query = new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(filter), startIndex, count);

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