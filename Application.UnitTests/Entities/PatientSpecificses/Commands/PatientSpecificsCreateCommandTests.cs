using Application.Entity.PatientSpecificses.Commands.Create;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace Application.UnitTests.Entities.PatientSpecificss.Commands
{
	public class PatientSpecificsCreateCommandTests : TestsTheoryData
	{
		private readonly PatientSpecificsCreateCommandHandler _handler;
		private readonly IPatientSpecificsRepository _repository;
		private readonly IPatientCardRepository _cardRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly PatientSpecifics _patientSpecifics;
		private readonly PatientCard _patientCardAssigned;
		private readonly PatientCard _patientCardNotAssigned;
		public PatientSpecificsCreateCommandTests()
		{
			_repository = Substitute.For<IPatientSpecificsRepository>();
			_cardRepository = Substitute.For<IPatientCardRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();

			_patientCardAssigned = Patient.Create(Username.Create("Fullname")).Value.Card;
			_patientCardNotAssigned = Patient.Create(Username.Create("Fullname")).Value.Card;
			_patientSpecifics = PatientSpecifics.Create("", "", "", "", _patientCardAssigned).Value;

			_handler = new PatientSpecificsCreateCommandHandler(_repository, _cardRepository, _unitOfWork);

			_repository.AddAsync(Arg.Any<Result<PatientSpecifics>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<PatientSpecifics>>());

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<PatientSpecifics>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<PatientSpecifics>>());

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<PatientSpecifics, bool>>>(expr =>
				expr.Compile()(_patientSpecifics) == true), Arg.Any<CancellationToken>())
				.Returns(_patientSpecifics);

			_cardRepository.GetByIdAsync(Arg.Is<Guid>(x => x == _patientCardAssigned.Id), Arg.Any<CancellationToken>())
				.Returns(_patientCardAssigned);

			_repository.GetByPredicateAsync(Arg.Is<Expression<Func<PatientSpecifics, bool>>>(expr =>
				expr.Compile()(_patientSpecifics) == false), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<PatientSpecifics>(PersistenceErrors.Entity<PatientSpecifics>.NotFound));

			_cardRepository.GetByIdAsync(Arg.Is<Guid>(x => x == _patientCardNotAssigned.Id), Arg.Any<CancellationToken>())
				.Returns(_patientCardNotAssigned);
		}

		[Fact]
		public async Task Handle_Should_ReturnError_WhenSpecificsExisting()
		{
			//Act
			var result = await _handler.Handle(new PatientSpecificsCreateCommand(_patientCardAssigned.Id, "", "", "", ""), default);

			//Assert
			result.Error.Code.Should().Be(ApplicationErrors.PatientSpecificsCreateCommand.AlreadyExists);
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidInput()
		{
			//Act
			var result = await _handler.Handle(new PatientSpecificsCreateCommand(_patientCardNotAssigned.Id, "", "", "", ""), default);

			//Assert
			result.IsSuccess.Should().Be(true);
		}
	}
}