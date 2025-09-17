using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
	public class ProcedureTypeResourceType : BaseEntity
	{
		public ProcedureTypeResourceType(Guid id) : base(id) { }

		public ProcedureTypeResourceType(Guid id, ProcedureType procedureType, ResourceType resourceType, uint requeredAmount) : base(id)
		{
			ProcedureType = procedureType;
			ResourceType = resourceType;
			ProcedureTypeId = procedureType.Id;
			ResourceTypeId = resourceType.Id;
			RequeredAmount = requeredAmount;
		}
		
		public uint RequeredAmount { get; set; }
		public Guid ProcedureTypeId { get; set; }
		public Guid ResourceTypeId { get; set; }


		[JsonIgnore]
		public ProcedureType ProcedureType { get; set; } = null!;

		[JsonIgnore]
		public ResourceType ResourceType { get; set; } = null!;

		public static Result<ProcedureTypeResourceType> Create(Result<ProcedureType> procedureType, Result<ResourceType> resourceType, uint requeredAmount)
		{
			if (procedureType.IsFailure) return procedureType.Error;
			if (resourceType.IsFailure) return resourceType.Error;

			return new ProcedureTypeResourceType(Guid.NewGuid(), procedureType.Value, resourceType.Value, requeredAmount);
		}

		public Result<ProcedureTypeResourceType> ChangeRequeredAmount(uint newRequeredAmount)
		{
			RequeredAmount = newRequeredAmount;

			return this;
		}
	}
}
