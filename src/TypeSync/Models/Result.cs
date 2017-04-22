namespace TypeSync.Models
{
    public class Result
    {       
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public string ErrorString()
        {
            return $"error {ErrorCode}: {ErrorMessage}";
        }

        public static Result CreateSuccess()
        {
            return new Result()
            {
                Success = true
            };
        }

        public static Result CreateError(string message)
        {
            return new Result()
            {
                Success = false,
                ErrorMessage = message
            };
        }
    }
}
