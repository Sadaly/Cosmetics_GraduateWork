﻿using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class UserFilter : IEntityFilter<User>
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public DateTime? RegistrationDateFrom { get; set; }
        public DateTime? RegistrationDateTo { get; set; }

        public Expression<Func<User, bool>> ToPredicate()
        {
            return user =>
                (string.IsNullOrWhiteSpace(Username) || user.Username.Value.Contains(Username)) &&
                (string.IsNullOrWhiteSpace(Email) || user.Email.Value.Contains(Email)) &&
                (!RegistrationDateFrom.HasValue || user.CreatedAt >= RegistrationDateFrom.Value) &&
                (!RegistrationDateTo.HasValue || user.CreatedAt <= RegistrationDateTo.Value);
        }
    }
}
