namespace Microservice.ApiWeb.Dto.Exceptions
{
    public class ExceptionDtoResponse
    {
        public string? ErrorMessage { get; set; }
        public int? StatusCode { get; set; }
        public string? ErrorType { get; set; }
        public string? Timestamp { get; set; }
        public IEnumerable<string>? Failures { get; set; }
    }
}
