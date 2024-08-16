using Microsoft.AspNetCore.Http;

namespace HttpContextCatcher
{
    public class ResponseCatcher
    {
        public int? StatusCode { get; set; }

        public string Body { get; set; }

        public string ContentType { get; set; }

        public ResponseCatcher(int statusCode,
                               string body,
                               string contentType)
        {
            StatusCode = statusCode;
            Body = body;
            ContentType = contentType;
        }
    }
}
