﻿using System.Collections.Generic;

namespace HttpContextCatcher
{
    /// <summary>
    /// request content from HttpContext.Request
    /// </summary>
    public class RequestCatcher
    {
        public string Path { get; set; }

        public string Method { get; set; }

        public object Body { get; set; }

        public Dictionary<string, string> Queries { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public RequestCatcher(string path,
                              string method,
                              object body,
                              Dictionary<string, string> queries,
                              Dictionary<string, string> headers)
        {
            Path = path;
            Method = method;
            Body = body;
            Queries = queries;
            Headers = headers;
        }
    }
}
