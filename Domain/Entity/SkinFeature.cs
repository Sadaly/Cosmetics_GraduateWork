using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class SkinFeature : TransitiveEntity<SkinFeatureType>
    {
        private SkinFeature(Guid id) : base(id) { }
        private SkinFeature(Guid id, Guid typeId, Guid patientCardId) : base(id, typeId)
        {
            PatientCardId = patientCardId;
        }
        public PatientCard PatientCard { get; set; } = null!;
        public Guid PatientCardId { get; set; }
    }
}
