using Application.Entity.Doctors.Commands.Create;
using Application.Entity.Patients.Commands.Create;
using Application.Entity.Patients.Queries;
using Application.Entity.Patients.Queries.Get;
using Application.Entity.Procedures.Commands.AssignDoctor;
using Application.Entity.Procedures.Commands.ChangeType;
using Application.Entity.Procedures.Commands.Create;
using Application.Entity.Procedures.Commands.RemoveDoctor;
using Application.Entity.Procedures.Commands.SoftDelete;
using Application.Entity.Procedures.Commands.UpdateDate;
using Application.Entity.Procedures.Queries;
using Application.Entity.Procedures.Queries.Get;
using Application.Entity.Procedures.Queries.GetAll;
using Application.Entity.ProcedureTypes.Commands.Create;
using Application.Entity.ReservedDates.Commands.Create;
using Domain.SupportData.Filters;

namespace WebApi.IntegrationTests.Tests
{
    public class ProcedureTests : BaseIntegrationTest
    {
        private static readonly string _typename = "Typename";
        private static readonly string _descr = "Description";
        private static readonly string _name = "Fullname";

        private readonly ProcedureTypeCreateCommand createType;
        private readonly ProcedureTypeCreateCommand createType1;
        private readonly ProcedureTypeCreateCommand createType2;

        private readonly DoctorCreateCommand createDoctor;


        private readonly PatientCreateCommand createPatient;
        public ProcedureTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            createDoctor = new(_name);

            createType = new(_typename, _descr);
            createType1 = new(_typename + "1", _descr);
            createType2 = new(_typename + "1", _descr);

            createPatient = new(_name);
        }

        [Fact]
        public async Task Create_ShouldAddProcedure_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId);

            //Act
            var id = await Sender.Send(create);

            //Assert
            Assert.True(dbContext.Procedures.FirstOrDefault(d => d.Id == id.Value && d.TypeId == typeId) != null);
        }

        [Fact]
        public async Task Create_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var command = new ProcedureCreateCommand(Guid.NewGuid(), Guid.NewGuid());

            //Act
            var id = await Sender.Send(command);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task Update_ShouldChangeTypeProcedure_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId1 = (await Sender.Send(createType1)).Value;
            var typeId2 = (await Sender.Send(createType2)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId1);
            var id = await Sender.Send(create);

            //Act
            var update = new ProcedureChangeTypeCommand(id.Value, typeId2);
            id = await Sender.Send(update);

            //Assert
            Assert.True(dbContext.Procedures.FirstOrDefault(d => d.Id == id.Value && d.TypeId == typeId2) != null);
        }

        [Fact]
        public async Task Update_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);

            //Act
            var update = new ProcedureChangeTypeCommand(id.Value, Guid.NewGuid());
            id = await Sender.Send(update);

            //Assert
            Assert.True(id.IsFailure);
        }
        [Fact]
        public async Task Assign_ShouldAssignProcedure_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var doctorId = (await Sender.Send(createDoctor)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);

            //Act
            var assign = new ProcedureAssignDoctorCommand(id.Value, doctorId);
            id = await Sender.Send(assign);

            //Assert
            Assert.True(dbContext.Procedures.FirstOrDefault(d => d.Id == id.Value && d.DoctorId == doctorId) != null);
        }

        [Fact]
        public async Task Assign_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);

            //Act
            var assign = new ProcedureAssignDoctorCommand(id.Value, Guid.NewGuid());
            id = await Sender.Send(assign);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task RemoveDoctor_ShouldRemoveDoctorProcedure_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var doctorId = (await Sender.Send(createDoctor)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);
            var assign = new ProcedureAssignDoctorCommand(id.Value, doctorId);
            await Sender.Send(assign);

            //Act
            var removeDoctor = new ProcedureRemoveDoctorCommand(id.Value);
            id = await Sender.Send(removeDoctor);

            //Assert
            Assert.True(dbContext.Procedures.FirstOrDefault(d => d.Id == id.Value && d.DoctorId == null) != null);
        }

        [Fact]
        public async Task RemoveDoctor_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);

            //Act
            var removeDoctor = new ProcedureRemoveDoctorCommand(id.Value);
            id = await Sender.Send(removeDoctor);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task UpdateDate_ShouldUpdateDateProcedure_WhenCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);

            //Act
            var updateDate = new ProcedureUpdateDateCommand(id.Value, DateTime.UtcNow.AddDays(1), 30);
            id = await Sender.Send(updateDate);

            //Assert
            Assert.True(dbContext.Procedures.FirstOrDefault(d => d.Id == id.Value && d.Duration == 30) != null);
        }

        [Fact]
        public async Task UpdateDate_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);
            await Sender.Send(new ReservedDateCreateCommand(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), Domain.Enums.ReservedDateType.DayOfWeekRestrict));

            //Act
            var updateDate = new ProcedureUpdateDateCommand(id.Value, DateTime.UtcNow.AddDays(1), 30);
            id = await Sender.Send(updateDate);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldRemoveProcedure_WhenExists()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);
            var command = new ProcedureSoftDeleteCommand(id.Value);

            //Act
            var remove = await Sender.Send(command);
            var query = new ProcedureGetQuery(ProcedureQueries.GetById(id.Value));
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task RemoveById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var command = new ProcedureSoftDeleteCommand(Guid.NewGuid());

            //Act
            var result = await Sender.Send(command);

            //Assert
            Assert.True(result.IsFailure);
        }
        [Fact]
        public async Task GetById_ShouldReturnProcedure_WhenExists()
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId = (await Sender.Send(createType)).Value;
            var create = new ProcedureCreateCommand(patientCardId, typeId);
            var id = await Sender.Send(create);
            var query = new ProcedureGetQuery(ProcedureQueries.GetById(id.Value));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.TypeId == typeId);
        }

        [Fact]
        public async Task GetById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var query = new ProcedureGetQuery(ProcedureQueries.GetById(Guid.NewGuid()));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Theory]
        [MemberData(nameof(CorrectFilterResults))]
        public async Task GetAllByFilter_ShouldReturnProcedure_WhenExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId1 = (await Sender.Send(createType1)).Value;
            var typeId2 = (await Sender.Send(createType2)).Value;
            var create1 = new ProcedureCreateCommand(patientCardId, typeId1);
            var create2 = new ProcedureCreateCommand(patientCardId, typeId2);
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new ProcedureFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                PatientName = name,
            };
            var query = new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(filter));

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
            var create1 = new ProcedureCreateCommand(patientCardId, typeId1);
            var create2 = new ProcedureCreateCommand(patientCardId, typeId2);
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new ProcedureFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                PatientName = name,
            };
            var query = new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(filter));

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
        public async Task Take_ShouldReturnProcedure_WhenCorrectIndexes(int startIndex, int count)
        {
            //Arrange
            var patientId = (await Sender.Send(createPatient)).Value;
            var patientCardId = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardtId;
            var typeId1 = (await Sender.Send(createType1)).Value;
            var typeId2 = (await Sender.Send(createType2)).Value;
            var create1 = new ProcedureCreateCommand(patientCardId, typeId1);
            var create2 = new ProcedureCreateCommand(patientCardId, typeId2);
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new ProcedureFilter();

            var query = new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(filter), startIndex, count);

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
            var create1 = new ProcedureCreateCommand(patientCardId, typeId1);
            var create2 = new ProcedureCreateCommand(patientCardId, typeId2);
            var id1 = await Sender.Send(create1);
            var id2 = await Sender.Send(create2);

            var filter = new ProcedureFilter();
            var query = new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(filter), startIndex, count);

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