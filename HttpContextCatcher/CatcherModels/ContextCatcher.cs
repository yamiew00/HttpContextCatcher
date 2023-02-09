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

        internal void SetResSecond(decimal resSecond)
        {
            //response will be null if json deserization failed.
            if (Response == null)
            {
                Response = new ResponseCatcher(resSecond);
                return;
            }
            else
            {
                Response.ResSecond = resSecond;
            }
        }
    }
}
