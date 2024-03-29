﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HttpContextCatcher
{
    internal static class HttpContextExtension
    {
        internal static Dictionary<string, string> GetRequestQueries(this HttpContext context)
        {
            return context.Request
                          .Query
                          .ToDictionary(item => item.Key,
                                        item => item.Value.ToString());
        }

        internal static Dictionary<string, string> GetRequestHeaders(this HttpContext context)
        {
            return context.Request
                          .Headers
                          .ToDictionary(item => item.Key,
                                        item => item.Value.ToString());
        }

        internal static async Task<string> GetJsonRequestBody(this HttpContext context)
        {
            var request = context.Request;
            request.EnableBuffering();
            var requestStream = new StreamReader(request.Body);
            var content = await requestStream.ReadToEndAsync();

            request.Body.Position = 0;
            return content;
        }
    }
}
