using Application.Entity.PatientCards.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.PatientCards.Commands
{
	public class PatientCardUpdateCommandTests : TestsTheoryData
	{
		private readonly PatientCardUpdateCommandHandler _handler;
		private readonly IPatientCardRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly PatientCard _patientcard;
		private readonly Patient _patient;

		public PatientCardUpdateCommandTests()
		{
			_repository = Substitute.For<IPatientCardRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_patient = Patient.Create(Username.Create("Fullname").Value).Value;
			_patientcard = _patient.Card;

			_handler = new PatientCardUpdateCommandHandler(_repository, _unitOfWork);
			_repository.UpdateAsync(Arg.Any<Result<PatientCard>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<PatientCard>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _patientcard.Id), Arg.Any<CancellationToken>())
				.Returns(_patientcard);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _patientcard.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<PatientCard>(PersistenceErrors.Entity<PatientCard>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<PatientCard>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<PatientCard>>());
		}

		[Theory]
		[MemberData(nameof(InvalidPatientCardUpdateTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string? Address, string? Complains, string? Phonenumber, string expectedErrorCode)
		{
			//Act
			var result = await _handler.Handle(new PatientCardUpdateCommand(_patientcard.Id, 0, Address, Complains, Phonenumber), default);

			//Assert
			result.Error.Code.Should().Be(expectedErrorCode);
		}

		[Theory]
		[MemberData(nameof(ValidPatientCardUpdateTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string? Address, string? Complains, string? Phonenumber)
		{
			//Act
			var result = await _handler.Handle(new PatientCardUpdateCommand(_patientcard.Id, 0, Address, Complains, Phonenumber), default);

			//Assert
			result.Value.Should().Be(_patientcard.Id);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new PatientCardUpdateCommand(Guid.Parse(id), 0, "", "", ""), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<PatientCard>.NotFound);
		}
	}
}