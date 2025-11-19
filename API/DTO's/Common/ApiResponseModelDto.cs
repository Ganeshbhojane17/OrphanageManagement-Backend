namespace Orphanage.API.DTO_s.Common
{
    public class ApiResponseModelDto<TResponse>
    {
        public bool IsSuccess { get; set; }

        public TResponse Data { get; set; }

        public string Message { get; set; }

        public ApiResponseModelDto(bool isSuccess, TResponse data, string message)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
        }
    }
}
