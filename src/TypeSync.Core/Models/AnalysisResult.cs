namespace TypeSync.Core.Models
{
    public class AnalysisResult<T> where T: class
    {
        public T Value { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}
