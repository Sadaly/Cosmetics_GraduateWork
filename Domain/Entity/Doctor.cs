using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
    public class Doctor : BaseEntity
    {
        private Doctor(Guid id) : base(id) { }
        private Doctor(Guid id, Username fullname) : base(id)
        {
            Fullname = fullname;
        }

        public Username Fullname { get; set; } = null!;
        [JsonIgnore]
        public List<Procedure> Procedures => _procedures;
        private readonly List<Procedure> _procedures = [];
        public static Result<Doctor> Create(Result<Username> fullname)
        {
            if (fullname.IsFailure) return Result.Failure<Doctor>(fullname.Error);
            return new Doctor(Guid.NewGuid(), fullname.Value);
        }

        public Result<Doctor> Update(Result<Username> name)
        {
            if (name.IsFailure) return Result.Failure<Doctor>(name.Error);
            this.Fullname = name.Value;
            return this;
        }
    }
}
