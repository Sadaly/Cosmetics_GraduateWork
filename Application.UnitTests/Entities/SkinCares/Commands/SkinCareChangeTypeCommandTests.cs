using Application.Entity.SkinCares.Commands.ChangeType;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.SkinCares.Commands
{
	public class SkinCareChangeTypeCommandTests : TestsTheoryData
	{
		private readonly SkinCareChangeTypeCommandHandler _handler;
		private readonly ISkinCareRepository _repository;
		private readonly ISkinCareTypeRepository _typeRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly Patient _patient;
		private readonly PatientCard _patientCard;
		private readonly SkinCareType _skinCareType;
		private readonly SkinCareType _skinCareTypeUpdated;
		private readonly SkinCare _skincare;


		public SkinCareChangeTypeCommandTests()
		{
			_repository = Substitute.For<ISkinCareRepository>();
			_typeRepository = Substitute.For<ISkinCareTypeRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_patient = Patient.Create(Username.Create("Fullname")).Value;
			_patientCard = _patient.Card;
			_skinCareType = SkinCareType.Create(Title.Create("Title")).Value;
			_skinCareTypeUpdated = SkinCareType.Create(Title.Create("Updated")).Value;
			_skincare = SkinCare.Create(_patientCard, _skinCareType).Value;
			_handler = new SkinCareChangeTypeCommandHandler(_repository, _typeRepository, _unitOfWork);

			_repository.UpdateAsync(Arg.Any<Result<SkinCare>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<SkinCare>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _skincare.Id), Arg.Any<CancellationToken>())
				.Returns(_skincare);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _skincare.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<SkinCare>(PersistenceErrors.Entity<SkinCare>.NotFound));

			_typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == _skinCareTypeUpdated.Id), Arg.Any<CancellationToken>())
				.Returns(_skinCareTypeUpdated);

			_typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != _skinCareTypeUpdated.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<SkinCareType>(PersistenceErrors.Entity<SkinCareType>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinCare>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<SkinCare>>());
		}

		[Theory]
		[MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
		public async Task Handle_Should_ReturnFailure_WhenInvalidGuids(string SkinCareId, string newTypeId)
		{
			//Act
			var result = await _handler.Handle(new SkinCareChangeTypeCommand(Guid.Parse(SkinCareId), Guid.Parse(newTypeId)), default);

			//Assert
			result.IsFailure.Should().Be(true);
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidGuids()
		{
			//Act
			var result = await _handler.Handle(new SkinCareChangeTypeCommand(_skincare.Id, _skinCareTypeUpdated.Id), default);

			//Assert
			result.Value.Should().Be(_skincare.Id);
		}
	}
}