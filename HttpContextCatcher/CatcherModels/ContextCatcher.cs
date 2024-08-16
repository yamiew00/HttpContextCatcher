using System;
using System.Collections.Generic;
using System.Text;

namespace HttpContextCatcher
{
    /// <summary>
    /// all content catched from HttpContext
    /// </summary>
    public class ContextCatcher
    {
        public DateTime Time { get; set; }

        public RequestCatcher Request { get; set; }

        public ResponseCatcher Response { get; set; }

        public ExceptionCatcher Exception { get; set; }

        public decimal CostSecond { get; internal set; }


        public ContextCatcher(DateTime time,
                              RequestCatcher request,
                              ResponseCatcher response,
                              ExceptionCatcher exception)
        {
            Time = time;
            Request = request;
            Response = response;
            Exception = exception;
        }
    }
}
