using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entity
{
    public class SkinFeatureType : TypeEntity
    {
        private SkinFeatureType(Guid id) : base(id) { }
        private SkinFeatureType(Guid id, Title title) : base(id, title)
        {
        }
    }
}
