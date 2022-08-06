namespace Microservice.Application.Services.Security.Interfaces
{
    public interface IHashProviderService
    {
        public string Hash(string password);
        public bool Check(string StoredPassword, string Password);
    }
}
