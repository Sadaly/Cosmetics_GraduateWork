using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
    public class ProcedureType : TypeEntity
    {
        private ProcedureType(Guid id) : base(id) { }
        private ProcedureType(Guid id, Title title, string description, int standartDuration) : base(id, title)
        {
            Description = description;
            StandartDuration = standartDuration;
        }
        public int StandartDuration { get; set; }
        public string Description { get; set; } = string.Empty;
        public static Result<ProcedureType> Create(Result<Title> title, string description, int standartDuration)
        {
            if (title.IsFailure) return Result.Failure<ProcedureType>(title);
            if (standartDuration < 0) return Result.Failure<ProcedureType>(DomainErrors.Procedure.DurationLessThenZero);
            return new ProcedureType(Guid.NewGuid(), title.Value, description, standartDuration);
        }
    }
}
