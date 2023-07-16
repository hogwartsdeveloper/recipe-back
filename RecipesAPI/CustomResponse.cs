namespace RecipesAPI
{
    public class CustomResponse
    {
        public int StatusCode { get; }
        public string Message { get; } 
        public string? Details { get; }

        public CustomResponse(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }
}

