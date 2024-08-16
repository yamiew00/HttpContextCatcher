using Microsoft.AspNetCore.Http;

namespace HttpContextCatcher
{
    public class ResponseCatcher
    {
        public int? StatusCode { get; set; }

        public string Body { get; set; }

        public ResponseCatcher(int statusCode,
                               string body)
        {
            StatusCode = statusCode;
            Body = body;
        }
    }
}
