namespace Orphanage.API.DTO_s.Common
{
    public class ApiErrorModelDto
    {
        public int StatusCode { get; set; }

        public string Title { get; set; }

        public string? Detail { get; set; }

        public string? StackTrace { get; set; }
    }
}
