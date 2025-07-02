namespace Domain.Common
{
    public class TransitiveEntity<T> : BaseEntity where T : TypeEntity
    {
        public TransitiveEntity(Guid id) : base(id) { }
        public TransitiveEntity(Guid id, Guid typeId) : base(id)
        {
            TypeId = typeId;
        }

        public Guid TypeId { get; set; }
        public T Type { get; set; } = null!;
    }
}
