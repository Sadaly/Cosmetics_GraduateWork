using Domain.Common;
using Domain.Enums;
using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
	public class ResourceOperation : BaseEntity
	{
		public ResourceOperation()
		{
		}

		public ResourceOperation(Guid id) : base(id)
		{
		}

		public ResourceOperation(Guid id, ResourceOperationEnumType enumType, uint changeAmount, uint resourcePrice, Resource? resource) : base(id)
		{
			ChangeAmount = changeAmount;
			ResourcePrice = resourcePrice;
			EnumType = enumType;
			Resource = resource;
			ResourceId = resource?.Id;
		}

		public uint ChangeAmount { get; set; }
		public uint ResourcePrice { get; set; }
		public ResourceOperationEnumType EnumType { get; set; } = ResourceOperationEnumType.None;

		[NotMapped]
		public uint TotalPrice => ChangeAmount * ResourcePrice;

		public Guid? ResourceId { get; set; }
		public Resource? Resource { get; set; } = null!;

		public static Result<ResourceOperation> Create(ResourceOperationEnumType enumType, uint changeAmount, uint resourcePrice, Resource? resource = null)
		{
			return new ResourceOperation(Guid.NewGuid(), enumType, changeAmount, resourcePrice, resource);
		}
	}
}
