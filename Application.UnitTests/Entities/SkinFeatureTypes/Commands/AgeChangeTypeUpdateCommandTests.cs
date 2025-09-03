using Application.Entity.SkinFeatureTypes.Commands.Update;
using Application.UnitTests.TheoryData;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Errors;
using Domain.Repositories;
using Domain.Shared;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Entities.SkinFeatureTypes.Commands
{
	public class SkinFeatureTypeUpdateCommandTests : TestsTheoryData
	{
		private readonly SkinFeatureTypeUpdateCommandHandler _handler;
		private readonly ISkinFeatureTypeRepository _repository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly SkinFeatureType _skinfeaturetype;

		public SkinFeatureTypeUpdateCommandTests()
		{
			_repository = Substitute.For<ISkinFeatureTypeRepository>();
			_unitOfWork = Substitute.For<IUnitOfWork>();
			_skinfeaturetype = SkinFeatureType.Create(Title.Create("Fullname").Value).Value;

			_handler = new SkinFeatureTypeUpdateCommandHandler(_repository, _unitOfWork);
			_repository.UpdateAsync(Arg.Any<Result<SkinFeatureType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<SkinFeatureType>>());

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x == _skinfeaturetype.Id), Arg.Any<CancellationToken>())
				.Returns(_skinfeaturetype);

			_repository.GetByIdAsync(Arg.Is<Guid>(x => x != _skinfeaturetype.Id), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<SkinFeatureType>(PersistenceErrors.Entity<SkinFeatureType>.NotFound));

			_unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinFeatureType>>(), Arg.Any<CancellationToken>())
				.Returns(c => c.Arg<Result<SkinFeatureType>>());
		}

		[Theory]
		[MemberData(nameof(InvalidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnError_WhenInvalidNameInput(string Name, string expectedErrorCode)
		{
			//Act
			var result = await _handler.Handle(new SkinFeatureTypeUpdateCommand(_skinfeaturetype.Id, Name), default);

			//Assert
			result.Error.Code.Should().Be(expectedErrorCode);
		}

		[Theory]
		[MemberData(nameof(ValidTitleCreationTestCases))]
		public async Task Handle_Should_ReturnSuccess_WhenValidNameInput(string Name)
		{
			//Act
			var result = await _handler.Handle(new SkinFeatureTypeUpdateCommand(_skinfeaturetype.Id, Name), default);

			//Assert
			result.Value.Should().Be(_skinfeaturetype.Id);
			_skinfeaturetype.Title.Should().Be(Title.Create(Name).Value);
		}

		[Theory]
		[MemberData(nameof(InvalidPrimaryKeyGetTestCases))]
		public async Task Handle_Should_ReturnError_WhenIdIsEmpty(string id)
		{
			//Act
			var result = await _handler.Handle(new SkinFeatureTypeUpdateCommand(Guid.Parse(id), _skinfeaturetype.Title.Value), default);

			//Assert
			result.Error.Should().Be(PersistenceErrors.Entity<SkinFeatureType>.NotFound);
		}
		[Fact]
		public async Task Handle_Should_ReturnError_WhenSaveIsFailer()
		{
			//Arrange
			_unitOfWork.SaveChangesAsync(Arg.Any<Result<SkinFeatureType>>(), Arg.Any<CancellationToken>())
				.Returns(Result.Failure<SkinFeatureType>(PersistenceErrors.Entity<SkinFeatureType>.NotFound));

			//Act
			var result = await _handler.Handle(new SkinFeatureTypeUpdateCommand(_skinfeaturetype.Id, _skinfeaturetype.Title.Value), default);

			//Assert
			result.IsFailure.Should().Be(true);
		}
	}
}