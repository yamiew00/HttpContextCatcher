using Microsoft.AspNetCore.Http;

namespace HttpContextCatcher
{
    public class ResponseCatcher
    {
        public string Body { get; set; }

        public string ContentType { get; set; }

        public ResponseCatcher(string body,
                               string contentType)
        {
            Body = body;
            ContentType = contentType;
        }
    }
}
