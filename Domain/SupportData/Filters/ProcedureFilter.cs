﻿using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class ProcedureFilter : IEntityFilter<Procedure>
    {
        public string? Typename { get; set; }
        public string? PatienName{ get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<Procedure, bool>> ToPredicate()
        {
            return procedure =>
                (string.IsNullOrWhiteSpace(Typename) || procedure.Type.Title.Value.Contains(Typename)) &&
                (string.IsNullOrWhiteSpace(PatienName) || procedure.PatientCard.Patient.Fullname.Value.Contains(PatienName)) &&
                (!CreationDateFrom.HasValue || procedure.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || procedure.CreatedAt <= CreationDateTo.Value);
        }
    }
}
