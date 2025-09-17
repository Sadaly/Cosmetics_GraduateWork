using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
namespace Domain.Entity
{
	public class ExternalProcedureRecordType : TypeEntity
	{
		private ExternalProcedureRecordType(Guid id) : base(id) { }
		private ExternalProcedureRecordType(Guid id, Title title) : base(id, title)
		{
		}
		public static Result<ExternalProcedureRecordType> Create(Result<Title> title)
		{
			if (title.IsFailure) return title.Error;
			return new ExternalProcedureRecordType(Guid.NewGuid(), title.Value);
		}
	}
}
