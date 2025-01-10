using Microsoft.AspNetCore.Http;
using System;

namespace HttpContextCatcher
{
    /// <summary>
    /// all content catched from HttpContext
    /// </summary>
    public class ContextCatcher
    {
        public DateTime Time { get; private set; }

        public RequestCatcher Request { get; private set; }

        public ResponseCatcher Response { get; private set; }

        public ExceptionCatcher Exception { get; private set; }

        public int StatusCode { get; private set; }

        public decimal CostSecond { get; private set; }

        public HttpContext HttpContext { get; private set; }

        public ContextCatcher(DateTime time,
                              RequestCatcher request,
                              ResponseCatcher response,
                              ExceptionCatcher exception,
                              int statusCode,
                              decimal costSecond,
                              HttpContext httpContext)
        {
            Time = time;
            Request = request;
            Response = response;
            Exception = exception;
            StatusCode = statusCode;
            CostSecond = costSecond;
            HttpContext = httpContext;
        }
    }
}
