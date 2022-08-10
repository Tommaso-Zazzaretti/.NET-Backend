namespace Microservice.ApiWeb.Dto.Filters
{
    public class FilterDtoStringPredicate
    {
        public string? StringFieldName { get; set; } // Example: "UserName", "Name"
        public string? Pattern { get; set; }         // Example: "New Y"
        public string? Action { get; set; }          // Example: "Contains", "StartsWith"
    }
}
