using System;
namespace HttpContextCatcher
{
    public class ExceptionCatcher
    {
        public string Type { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public ExceptionCatcher(Exception exception)
        {
            Type = exception.GetType().ToString();
            Message = exception.Message;
            StackTrace = exception.StackTrace;
        }
    }
}
