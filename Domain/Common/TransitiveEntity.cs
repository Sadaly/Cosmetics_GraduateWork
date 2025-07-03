namespace Domain.Common
{
    public class TransitiveEntity<T> : BaseEntity where T : TypeEntity
    {
        public TransitiveEntity(Guid id) : base(id) { }
        public TransitiveEntity(Guid id, T type) : base(id)
        {
            TypeId = type.Id;
            Type = type;
        }

        public Guid TypeId { get; set; }
        public T Type { get; set; } = null!;
    }
}
