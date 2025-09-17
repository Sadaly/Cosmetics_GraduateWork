using Domain.Common;
using Domain.Shared;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
	public class ExternalProcedureRecord : EntityWithType<ExternalProcedureRecordType>
	{
		private ExternalProcedureRecord(Guid id) : base(id) { }
		private ExternalProcedureRecord(Guid id, PatientCard patientCard, ExternalProcedureRecordType type, string date) : base(id, type)
		{
			PatientCardId = patientCard.Id;
			PatientCard = patientCard;
			Date = date;
		}
		[JsonIgnore]
		public PatientCard PatientCard { get; set; } = null!;
		public Guid PatientCardId { get; set; }
		public string Date { get; set; } = null!;


		public static Result<ExternalProcedureRecord> Create(Result<PatientCard> patientCard, Result<ExternalProcedureRecordType> type, string date = "")
		{
			if (patientCard.IsFailure) return patientCard.Error;
			if (type.IsFailure) return type.Error;

			return new ExternalProcedureRecord(Guid.NewGuid(), patientCard.Value, type.Value, date);
		}
	}
}
