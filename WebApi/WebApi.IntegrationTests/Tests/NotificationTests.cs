using Application.Entity.Notifications.Commands.Create;
using Application.Entity.Notifications.Commands.SoftDelete;
using Application.Entity.Notifications.Commands.UpdateMessage;
using Application.Entity.Notifications.Commands.UpdatePhoneNumber;
using Application.Entity.Notifications.Queries;
using Application.Entity.Notifications.Queries.Get;
using Application.Entity.Notifications.Queries.GetAll;
using Application.Entity.Patients.Commands.Create;
using Application.Entity.Patients.Queries;
using Application.Entity.Patients.Queries.Get;
using Application.Entity.Procedures.Commands.Create;
using Application.Entity.ProcedureTypes.Commands.Create;
using Domain.SupportData.Filters;

namespace WebApi.IntegrationTests.Tests
{
    public class NotificationTests : BaseIntegrationTest
    {
        private readonly static string _typename = "Typename";
        private readonly static string _typeDesr = "Description";
        private readonly static string _message = "Message";
        private readonly static string _name = "Fullname";

        private readonly ProcedureTypeCreateCommand createType;
        private readonly ProcedureTypeCreateCommand createType1;
        private readonly ProcedureTypeCreateCommand createType2;

        private readonly PatientCreateCommand createPatient;
        public NotificationTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            createType = new(_typename, _typeDesr);
            createType1 = new(_typename + "1", _typeDesr);
            createType2 = new(_typename + "1", _typeDesr);

            createPatient = new(_name + "1");
        }

        [Fact]
        public async Task Create_ShouldAddNotification_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var procedureId = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId, 100, null, DateTime.UtcNow))).Value;
            var create = new NotificationCreateCommand(procedureId, _message, DateTime.UtcNow.AddDays(-1));

            //Act
            var id = await Sender.Send(create);

            //Assert
            Assert.True(dbContext.Notifications.FirstOrDefault(d => d.Id == id.Value) != null);
        }

        [Fact]
        public async Task Create_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var command = new NotificationCreateCommand(Guid.NewGuid(), _message, DateTime.UtcNow.AddDays(-1));

            //Act
            var id = await Sender.Send(command);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task UpdateMessage_ShouldUpdateMessageNotification_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var procedureId = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId, 100, null, DateTime.UtcNow))).Value;
            var create = new NotificationCreateCommand(procedureId, _message, DateTime.UtcNow.AddDays(-1));
            var id = await Sender.Send(create);

            //Act
            var update = new NotificationUpdateMessageCommand(id.Value, _message + "aaa");
            id = await Sender.Send(update);

            //Assert
            Assert.True(dbContext.Notifications.FirstOrDefault(d => d.Id == id.Value && d.Message.Value == update.Message) != null);
        }

        [Fact]
        public async Task UpdateMessage_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var procedureId = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId, 100, null, DateTime.UtcNow))).Value;
            var create = new NotificationCreateCommand(procedureId, _message, DateTime.UtcNow.AddDays(-1));
            var id = await Sender.Send(create);

            //Act
            var update = new NotificationUpdateMessageCommand(id.Value, "");
            id = await Sender.Send(update);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task UpdatePhonenumber_ShouldUpdatePhonenumberNotification_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var procedureId = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId, 100, null, DateTime.UtcNow))).Value;
            var create = new NotificationCreateCommand(procedureId, _message, DateTime.UtcNow.AddDays(-1));
            var id = await Sender.Send(create);

            //Act
            var update = new NotificationUpdatePhoneNumberCommand(id.Value, "281930189");
            id = await Sender.Send(update);

            //Assert
            Assert.True(dbContext.Notifications.FirstOrDefault(d => d.Id == id.Value && d.PhoneNumber.Value == update.PhoneNumber) != null);
        }

        [Fact]
        public async Task UpdatePhonenumber_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var procedureId = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId, 100, null, DateTime.UtcNow))).Value;
            var create = new NotificationCreateCommand(procedureId, _message, DateTime.UtcNow.AddDays(-1));
            var id = await Sender.Send(create);

            //Act
            var update = new NotificationUpdatePhoneNumberCommand(id.Value, "");
            id = await Sender.Send(update);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldRemoveNotification_WhenExists()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var procedureId = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId, 100, null, DateTime.UtcNow))).Value;
            var create = new NotificationCreateCommand(procedureId, _message, DateTime.UtcNow.AddDays(-1));
            var id = await Sender.Send(create);

            var command = new NotificationSoftDeleteCommand(id.Value);

            //Act
            var remove = await Sender.Send(command);
            var query = new NotificationGetQuery(NotificationQueries.GetById(id.Value));
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var command = new NotificationSoftDeleteCommand(Guid.NewGuid());

            //Act
            var result = await Sender.Send(command);

            //Assert
            Assert.True(result.IsFailure);
        }
        [Fact]
        public async Task GetById_ShouldReturnNotification_WhenExists()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var procedureId = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId, 100, null, DateTime.UtcNow))).Value;
            var create = new NotificationCreateCommand(procedureId, _message, DateTime.UtcNow.AddDays(-1));
            var id = await Sender.Send(create);

            var query = new NotificationGetQuery(NotificationQueries.GetById(id.Value));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.Id == id.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var query = new NotificationGetQuery(NotificationQueries.GetById(Guid.NewGuid()));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Theory]
        [MemberData(nameof(CorrectFilterResults))]
        public async Task GetAllByFilter_ShouldReturnNotification_WhenExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId1 = (await Sender.Send(createType1)).Value;
            var typeId2 = (await Sender.Send(createType2)).Value;
            var procedureId1 = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId1, 100, null, DateTime.UtcNow))).Value;
            var procedureId2 = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId2, 100, null, DateTime.UtcNow))).Value;
            var create1 = new NotificationCreateCommand(procedureId1, _message, DateTime.UtcNow.AddDays(-1));
            var create2 = new NotificationCreateCommand(procedureId2, _message, DateTime.UtcNow.AddDays(-1));
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new NotificationFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                PatientName = name,
            };
            var query = new NotificationGetAllQuery(NotificationQueries.GetByFilter(filter));

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
            var procedureId1 = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId1, 100, null, DateTime.UtcNow))).Value;
            var procedureId2 = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId2, 100, null, DateTime.UtcNow))).Value;
            var create1 = new NotificationCreateCommand(procedureId1, _message, DateTime.UtcNow.AddHours(-1));
            var create2 = new NotificationCreateCommand(procedureId2, _message, DateTime.UtcNow.AddHours(-1));
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new NotificationFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                PatientName = name,
            };
            var query = new NotificationGetAllQuery(NotificationQueries.GetByFilter(filter));

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
            { null, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(-1) },
            { _name + "3", null, null },
            { _name + "3", null, DateTime.UtcNow.AddDays(-1) },
            { _name + "3", DateTime.UtcNow.AddDays(-1), null },
            { _name + "3", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(-1) },
        };



        [Theory]
        [MemberData(nameof(CorrectTakeDataResults))]
        public async Task Take_ShouldReturnNotification_WhenCorrectIndexes(int startIndex, int count)
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId1 = (await Sender.Send(createType1)).Value;
            var typeId2 = (await Sender.Send(createType2)).Value;
            var procedureId1 = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId1, 100, null, DateTime.UtcNow))).Value;
            var procedureId2 = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId2, 100, null, DateTime.UtcNow))).Value;
            var create1 = new NotificationCreateCommand(procedureId1, _message, DateTime.UtcNow.AddDays(-1));
            var create2 = new NotificationCreateCommand(procedureId2, _message, DateTime.UtcNow.AddDays(-1));
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new NotificationFilter();

            var query = new NotificationGetAllQuery(NotificationQueries.GetByFilter(filter), startIndex, count);

            //Act
            var result = await Sender.Send(query);
            var w = result.Value.Count;
            Console.WriteLine(w);
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
            var procedureId1 = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId1, 100, null, DateTime.UtcNow))).Value;
            var procedureId2 = (await Sender.Send(new ProcedureCreateCommand(patientCardId, typeId2, 100, null, DateTime.UtcNow))).Value;
            var create1 = new NotificationCreateCommand(procedureId1, _message, DateTime.UtcNow.AddDays(-1));
            var create2 = new NotificationCreateCommand(procedureId2, _message, DateTime.UtcNow.AddDays(-1));
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new NotificationFilter();
            var query = new NotificationGetAllQuery(NotificationQueries.GetByFilter(filter), startIndex, count);

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