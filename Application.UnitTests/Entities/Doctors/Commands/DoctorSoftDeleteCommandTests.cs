using Application.Entity.Doctors.Commands.SoftDelete;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.Doctors.Commands
{
	public class DoctorSoftDeleteCommandTests : TestsTheoryData
	{
		private readonly DoctorSoftDeleteCommandHandler _handler;
		private readonly IDoctorRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly Doctor _doctor;

		public DoctorSoftDeleteCommandTests()
		{
			_repository = Substitute.For<IDoctorRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_doctor = Doctor.Create(Username.Create("Fullname").Value).Value;

			_handler = new DoctorSoftDeleteCommandHandler(_repository, _unitOfWork);

			_repository.RemoveAsync(Arg.Any<Result<Doctor>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<Doctor>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _doctor.Id), Arg.Any<CancellationToken>())
				.Returns(_doctor);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _doctor.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<Doctor>(PersistenceErrors.Entity<Doctor>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<Doctor>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<Doctor>>());
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidId()
		{
			//Act
			var result = await _handler.Handle(new DoctorSoftDeleteCommand(_doctor.Id), default);

			//Assert
			result.Value.Should().Be(_doctor.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new DoctorSoftDeleteCommand(Guid.Parse(id)), default);

			result.Error.Should().Be(PersistenceErrors.Entity<Doctor>.NotFound);
		}
	}
}