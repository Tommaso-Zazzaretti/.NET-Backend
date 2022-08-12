namespace Microservice.ApiWeb.Dto.Filters
{
    public class EqualityFilterDto<T> where T : class
    {
        public string? StringFieldName { get; set; } // Example: "UserName", "Name"
        public T? EqualsConst { get; set; }
    }
}
