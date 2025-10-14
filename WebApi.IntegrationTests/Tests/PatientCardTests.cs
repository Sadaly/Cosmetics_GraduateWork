using Application.Entity.PatientCards.Commands.Update;
using Application.Entity.PatientCards.Queries;
using Application.Entity.PatientCards.Queries.Get;
using Application.Entity.PatientCards.Queries.GetAll;
using Application.Entity.Patients.Commands.Create;
using Application.Entity.Patients.Queries;
using Application.Entity.Patients.Queries.Get;
using Domain.SupportData.Filters;

namespace WebApi.IntegrationTests.Tests
{
	public class Patientcardests : BaseIntegrationTest
	{
		private static readonly string _name = "Fullname";

		private readonly PatientCreateCommand createPatient;
		private readonly PatientCreateCommand createPatient1;
		private readonly PatientCreateCommand createPatient2;

		public Patientcardests(IntegrationTestWebAppFactory factory) : base(factory)
		{
			createPatient = new(_name);
			createPatient1 = new(_name + "1");
			createPatient2 = new(_name + "1");
		}

		[Fact]
		public async Task Create_ShouldAddPatientCard_WhenCorrectInput()
		{
			//Arrange
			var patientId = (await Sender.Send(createPatient)).Value;

			//Act
			var id = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardId;

			//Assert
			Assert.True(dbContext.PatientCards.FirstOrDefault(d => d.Id == id) != null);
		}

		[Fact]
		public async Task Update_ShouldUpdatePatientCard_WhenCorrectInput()
		{
			//Arrange
			var patientId = (await Sender.Send(createPatient)).Value;
			var id = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardId;

			//Act
			var update = new PatientCardUpdateCommand(id, 20, "", "", "");
			id = (await Sender.Send(update)).Value;

			//Assert
			Assert.True(dbContext.PatientCards.FirstOrDefault(d => d.Id == id) != null);
		}

		[Fact]
		public async Task Update_ShouldReturnFailer_WhenInCorrectInput()
		{
			//Arrange
			var patientId = (await Sender.Send(createPatient)).Value;
			var id = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardId;

			//Act
			var command = new PatientCardUpdateCommand(id, 20, "", "", "asdasa2");
			var update = await Sender.Send(command);

			//Assert
			Assert.True(update.IsFailure);
		}

		[Fact]
		public async Task GetById_ShouldReturnPatientCard_WhenExists()
		{
			//Arrange
			var patientId = (await Sender.Send(createPatient)).Value;
			var id = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId)))).Value.CardId;
			var query = new PatientCardGetQuery(PatientCardQueries.GetById(id));

			//Act
			var result = await Sender.Send(query);

			//Assert
			Assert.True(result.Value.Id == id);
		}

		[Fact]
		public async Task GetById_ShouldReturnFailer_WhenNotExists()
		{
			//Arrange
			var query = new PatientCardGetQuery(PatientCardQueries.GetById(Guid.NewGuid()));

			//Act
			var result = await Sender.Send(query);

			//Assert
			Assert.True(result.IsFailure);
		}

		[Theory]
		[MemberData(nameof(CorrectFilterResults))]
		public async Task GetAllByFilter_ShouldReturnPatientCard_WhenExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
		{
			//Arrange
			var patientId1 = (await Sender.Send(createPatient1)).Value;
			var patientId2 = (await Sender.Send(createPatient2)).Value;
			var patientCardId1 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId1)))).Value.CardId;
			var patientCardId2 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId2)))).Value.CardId;

			var filter = new PatientCardFilter()
			{
				CreationDateFrom = creationDateFrom,
				CreationDateTo = creationDateTo,
				PatientName = name,
			};
			var query = new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(filter));

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
			var patientId2 = (await Sender.Send(createPatient2)).Value;
			var patientCardId1 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId1)))).Value.CardId;
			var patientCardId2 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId2)))).Value.CardId;

			var filter = new PatientCardFilter()
			{
				CreationDateFrom = creationDateFrom,
				CreationDateTo = creationDateTo,
				PatientName = name,
			};
			var query = new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(filter));

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
		public async Task Take_ShouldReturnPatientCard_WhenCorrectIndexes(int startIndex, int count)
		{
			//Arrange
			var patientId1 = (await Sender.Send(createPatient1)).Value;
			var patientId2 = (await Sender.Send(createPatient2)).Value;
			var patientCardId1 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId1)))).Value.CardId;
			var patientCardId2 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId2)))).Value.CardId;

			var filter = new PatientCardFilter();

			var query = new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(filter), startIndex, count);

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
			var patientId2 = (await Sender.Send(createPatient2)).Value;
			var patientCardId1 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId1)))).Value.CardId;
			var patientCardId2 = (await Sender.Send(new PatientGetQuery(PatientQueries.GetById(patientId2)))).Value.CardId;

			var filter = new PatientCardFilter();
			var query = new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(filter), startIndex, count);

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