namespace Microservice.Application.Services.Authorization
{
    public static class AuthorizationPolicies
    {
        public const string USER        = "IsUser";
        public const string ADMIN       = "IsAdmin";
        public const string SUPER_ADMIN = "IsSuperAdmin";
    }
}
