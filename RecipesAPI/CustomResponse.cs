namespace RecipesAPI
{
    public class CustomResponse
    {
        private int StatusCode { get; }
        private string Message { get; }
        private string? Details { get; }

        public CustomResponse(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }
}

