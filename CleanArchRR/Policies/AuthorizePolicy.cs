namespace WebApi.Policies
{
    public static class AuthorizePolicy
    {
        public const string UserOnly = nameof(UserOnly);
        public const string ClientOnly = nameof(ClientOnly);
    }
}
