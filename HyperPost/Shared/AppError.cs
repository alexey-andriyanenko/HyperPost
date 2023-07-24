using System.Collections.Generic;

namespace HyperPost.Shared
{
    public class AppError
    {
        public string Type { get; set; }
        public string? Message { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }

        public AppError(string type, string? message = null)
        {
            Type = type;
            Message = message;
        }
    }
}
