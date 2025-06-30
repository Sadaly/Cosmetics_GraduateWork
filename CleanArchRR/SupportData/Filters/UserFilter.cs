using Domain.Entity;
using System.Linq.Expressions;
using WebApi.Abstractions;

namespace WebApi.SupportData.Filters
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
                (string.IsNullOrEmpty(Username) || user.Username.Value.Contains(Username)) &&
                (string.IsNullOrEmpty(Email) || user.Email.Value.Contains(Email)) &&
                (!RegistrationDateFrom.HasValue || user.CreatedAt >= RegistrationDateFrom.Value) &&
                (!RegistrationDateTo.HasValue || user.CreatedAt <= RegistrationDateTo.Value);
        }
    }
}
