using Application.Entity.SkinFeatures.Commands.ChangeType;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.SkinFeatures.Commands
{
	public class SkinFeatureChangeTypeCommandTests : TestsTheoryData
	{
		private readonly SkinFeatureChangeTypeCommandHandler _handler;
		private readonly ISkinFeatureRepository _repository;
		private readonly ISkinFeatureTypeRepository _typeRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly Patient _patient;
		private readonly PatientCard _patientCard;
		private readonly SkinFeatureType _skinFeatureType;
		private readonly SkinFeatureType _skinFeatureTypeUpdated;
		private readonly SkinFeature _skinfeature;


		public SkinFeatureChangeTypeCommandTests()
		{
			_repository = Substitute.For<ISkinFeatureRepository>();
			_typeRepository = Substitute.For<ISkinFeatureTypeRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_patient = Patient.Create(Username.Create("Fullname")).Value;
			_patientCard = _patient.Card;
			_skinFeatureType = SkinFeatureType.Create(Title.Create("Title")).Value;
			_skinFeatureTypeUpdated = SkinFeatureType.Create(Title.Create("Updated")).Value;
			_skinfeature = SkinFeature.Create(_patientCard, _skinFeatureType).Value;
			_handler = new SkinFeatureChangeTypeCommandHandler(_repository, _typeRepository, _unitOfWork);

			_repository.UpdateAsync(Arg.Any<Result<SkinFeature>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<SkinFeature>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _skinfeature.Id), Arg.Any<CancellationToken>())
				.Returns(_skinfeature);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _skinfeature.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<SkinFeature>(PersistenceErrors.Entity<SkinFeature>.NotFound));

			_typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x == _skinFeatureTypeUpdated.Id), Arg.Any<CancellationToken>())
				.Returns(_skinFeatureTypeUpdated);

			_typeRepository.GetByIdAsync(Arg.Is<Guid>(x => x != _skinFeatureTypeUpdated.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<SkinFeatureType>(PersistenceErrors.Entity<SkinFeatureType>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinFeature>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<SkinFeature>>());
		}

		[Theory]
		[MemberData(nameof(InvalidEntityWithTypeGuidsTestCases))]
		public async Task Handle_Should_ReturnFailure_WhenInvalidGuids(string SkinFeatureId, string newTypeId)
		{
			//Act
			var result = await _handler.Handle(new SkinFeatureChangeTypeCommand(Guid.Parse(SkinFeatureId), Guid.Parse(newTypeId)), default);

			//Assert
			result.IsFailure.Should().Be(true);
		}

		[Fact]
		public async Task Handle_Should_ReturnSuccess_WhenValidGuids()
		{
			//Act
			var result = await _handler.Handle(new SkinFeatureChangeTypeCommand(_skinfeature.Id, _skinFeatureTypeUpdated.Id), default);

			//Assert
			result.Value.Should().Be(_skinfeature.Id);
		}
	}
}