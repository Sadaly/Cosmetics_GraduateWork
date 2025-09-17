using Domain.Common;
using Domain.Errors;
using Domain.Shared;

namespace Domain.Entity
{
	public class Resource : EntityWithType<ResourceType>
	{
		public Resource(Guid id) : base(id)
		{
		}

		public Resource(Guid id, uint price, uint amount) : base(id)
		{
			Price = price;
			Amount = amount;
		}

		public uint Price { get; set; }
		public uint Amount { get; set; }


		public static Result<Resource> Create(Result<ResourceType> type, uint price, uint amount)
		{
			if (type.IsFailure) return type.Error;

			return new Resource(Guid.NewGuid(), price, amount);
		}

		public Result<Resource> TakeAmount(uint take)
		{
			if (take < 0) return DomainErrors.Resource.AmountTakeZero;
			if (take > Amount) return DomainErrors.Resource.AmountTakeMoreThenHave;
			Amount = Amount - take;

			return this;
		}

		public Result<Resource> AddAmount(uint add)
		{
			if (add == 0) return DomainErrors.Resource.AmountAddZero;
			if (uint.MaxValue - Amount < add) return DomainErrors.Resource.AmountAddZero;
			Amount = Amount - add;

			return this;
		}
	}
}
