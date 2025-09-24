using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
	public class ProcedureType : TypeEntity
	{
		private ProcedureType(Guid id) : base(id) { }
		private ProcedureType(Guid id, Title title, string standartDescription, int standartDuration, int standartPrice) : base(id, title)
		{
			StandartDescription = standartDescription;
			StandartDuration = standartDuration;
		}
		public int StandartDuration { get; set; }
		public string StandartDescription { get; set; } = string.Empty;
		public int StandartPrice {  get; set; }
		public List<ProcedureTypeResourceType> ResourceTypes { get; } = [];

		public static Result<ProcedureType> Create(Result<Title> title, string standartDescription, int standartDuration, int standartPrice)
		{
			if (title.IsFailure) return title.Error;
			if (standartDuration < 0) return DomainErrors.Procedure.DurationLessThenZero;
			if (standartPrice < 0) return DomainErrors.Procedure.PriceLessThenZero;
			return new ProcedureType(Guid.NewGuid(), title.Value, standartDescription, standartDuration, standartPrice);
		}
		public Result<ProcedureType> UpdateDescription(string descr)
		{
			if (descr != null) StandartDescription = descr;
			return this;
		}
		public Result<ProcedureType> UpdateStandartDuration(int? dur)
		{
			if (dur != null) StandartDuration = dur.Value;
			return this;
		}
		public Result<ProcedureType> UpdateStandartPrice(int? dur)
		{
			if (dur != null) StandartPrice = dur.Value;
			return this;
		}
	}
}
