using System.Text.Json.Serialization;

namespace Domain.Common
{
    public class EntityWithTntity<T> : BaseEntity where T : TypeEntity
    {
        public EntityWithTntity(Guid id) : base(id) { }
        public EntityWithTntity(Guid id, T type) : base(id)
        {
            TypeId = type.Id;
            Type = type;
        }

        public Guid TypeId { get; set; }
        [JsonIgnore]
        public T Type { get; set; } = null!;
    }
}
