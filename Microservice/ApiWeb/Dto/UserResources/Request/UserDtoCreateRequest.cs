namespace Microservice.ApiWeb.Dto.UserResources.Request
{
    public class UserDtoCreateRequest
    {
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
