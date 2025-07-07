using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
    public class Doctor : BaseEntity
    {
        private Doctor(Guid id, Username fullname) : base(id)
        {
            Fullname = fullname;
        }
        public Username Fullname { get; set; }
        [JsonIgnore]
        public List<Procedure> Procedures => _procedures;
        private readonly List<Procedure> _procedures = [];
        public static Result<Doctor> Create(Result<Username> fullname)
        {
            if (fullname.IsFailure) return Result.Failure<Doctor>(fullname.Error);
            return new Doctor(Guid.NewGuid(), fullname.Value);
        }
    }
}
