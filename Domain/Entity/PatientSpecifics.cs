using Domain.Common;
using Domain.Shared;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
	public class PatientSpecifics : BaseEntity
	{
		private PatientSpecifics(Guid id) : base(id) { }
		private PatientSpecifics(Guid id, string sleep, string diet, string sport, string workEnviroment, PatientCard patientCard) : base(id)
		{
			Sleep = sleep;
			Diet = diet;
			Sport = sport;
			WorkEnviroment = workEnviroment;
			PatientCard = patientCard;
			PatientCardId = patientCard.Id;
		}
		public Guid PatientCardId { get; set; }
		[JsonIgnore]
		public PatientCard PatientCard { get; set; } = null!;
		public string Sleep { get; set; } = String.Empty;
		public string Diet { get; set; } = String.Empty;
		public string Sport { get; set; } = String.Empty;
		public string WorkEnviroment { get; set; } = String.Empty;

		public static Result<PatientSpecifics> Create(string sleep, string diet, string sport, string workEnviroment, Result<PatientCard> patientCard)
		{
			if (patientCard.IsFailure) return Result.Failure<PatientSpecifics>(patientCard);
			return new PatientSpecifics(Guid.NewGuid(), sleep, diet, sport, workEnviroment, patientCard.Value);
		}

		public Result<PatientSpecifics> Update(string sleep, string diet, string sport, string workEnviroment)
		{
			if (!string.IsNullOrWhiteSpace(sleep)) Sleep = sleep;
			if (!string.IsNullOrWhiteSpace(diet)) Diet = diet;
			if (!string.IsNullOrWhiteSpace(sport)) Sport = sport;
			if (!string.IsNullOrWhiteSpace(workEnviroment)) WorkEnviroment = workEnviroment;
			return this;
		}

	}
}
