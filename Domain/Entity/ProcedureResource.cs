using Domain.Common;
using Domain.Shared;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
	public class ProcedureResource : BaseEntity
	{
		public ProcedureResource()
		{
		}

		public ProcedureResource(Guid id) : base(id)
		{
		}

		public ProcedureResource(Guid id, Procedure procedure, Resource resource, uint requeredAmount) : base(id)
		{
			Procedure = procedure;
			Resource = resource;
			ProcedureId = procedure.Id;
			ResourceId = resource.Id;
			Amount = requeredAmount;
		}

		public uint Amount { get; set; }
		public Guid ProcedureId { get; set; }
		public Guid ResourceId { get; set; }


		[JsonIgnore]
		public Procedure Procedure { get; set; } = null!;

		[JsonIgnore]
		public Resource Resource { get; set; } = null!;

		public static Result<ProcedureResource> Create(Result<Procedure> procedure, Result<Resource> resource, uint requeredAmount)
		{
			if (procedure.IsFailure) return procedure.Error;
			if (resource.IsFailure) return resource.Error;

			return new ProcedureResource(Guid.NewGuid(), procedure.Value, resource.Value, requeredAmount);
		}
	}
}
