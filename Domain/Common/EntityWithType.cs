using System.Text.Json.Serialization;

namespace Domain.Common
{
    public class EntityWithType<T> : BaseEntity where T : TypeEntity
    {
        public EntityWithType(Guid id) : base(id) { }
        public EntityWithType(Guid id, T type) : base(id)
        {
            TypeId = type.Id;
            Type = type;
        }

        public Guid TypeId { get; set; }
        [JsonIgnore]
        public T Type { get; set; } = null!;
    }
}
