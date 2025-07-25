using Application.Entity.Doctors.Commands.Create;
using Application.Entity.Doctors.Queries;
using Application.Entity.Doctors.Queries.Get;
using Application.Entity.Doctors.Queries.GetAll;
using Domain.SupportData.Filters;

namespace Application.IntegrationTests.Tests
{
    public class DoctorTests : BaseIntegrationTest
    {
        private readonly static string _name = "Fullname";
        public DoctorTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Create_ShouldAddDoctor_WhenCorrectInput()
        {
            //Arrange
            var command = new DoctorCreateCommand(_name);

            //Act
            var id = await Sender.Send(command);

            //Assert
            Assert.True(dbContext.Doctors.FirstOrDefault(d => d.Id == id.Value) != null);
        }

        [Fact]
        public async Task Create_ShouldReturnFailer_WhenInCorrectInput()
        {
            //Arrange
            var command = new DoctorCreateCommand("");

            //Act
            var id = await Sender.Send(command);

            //Assert
            Assert.True(id.IsFailure);
        }

        [Fact]
        public async Task GetById_ShouldReturnDoctor_WhenExists()
        {
            //Arrange
            var command = new DoctorCreateCommand(_name);
            var id = await Sender.Send(command);
            var query = new DoctorGetQuery(DoctorQueries.GetById(id.Value));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.Value.Id == id.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnFailer_WhenNotExists()
        {
            //Arrange
            var query = new DoctorGetQuery(DoctorQueries.GetById(Guid.NewGuid()));

            //Act
            var result = await Sender.Send(query);

            //Assert
            Assert.True(result.IsFailure);
        }

        [Theory]
        [MemberData(nameof(CorrectFilterResults))]
        public async Task GetAllByFilter_ShouldReturnDoctor_WhenExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
        {
            //Arrange
            var command1 = new DoctorCreateCommand(_name + "1");
            var command2 = new DoctorCreateCommand(_name + "2");

            var id1 = await Sender.Send(command1);
            var id2 = await Sender.Send(command2);

            var filter = new DoctorFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                Name = name,
            };
            var query = new DoctorGetAllQuery(DoctorQueries.GetByFilter(filter));

            //Act
            var result = await Sender.Send(query);

            Console.WriteLine(result.Value.Count);

            //Assert
            Assert.True(result.Value.Count == 2);
        }

        [Theory]
        [MemberData(nameof(InCorrectFilterResults))]
        public async Task GetByAllFilter_ShouldReturnNone_WhenNotExists(string? name, DateTime? creationDateFrom, DateTime? creationDateTo)
        {
            //Arrange
            var command1 = new DoctorCreateCommand(_name + "1");
            var command2 = new DoctorCreateCommand(_name + "2");

            var filter = new DoctorFilter()
            {
                CreationDateFrom = creationDateFrom,
                CreationDateTo = creationDateTo,
                Name = name,
            };
            var query = new DoctorGetAllQuery(DoctorQueries.GetByFilter(filter));

            //Act
            var result = await Sender.Send(query);

            Console.WriteLine(result.Value.Count);
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
        public static TheoryData<string?, DateTime?, DateTime?> InCorrectFilterResults = new()
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
    }
}