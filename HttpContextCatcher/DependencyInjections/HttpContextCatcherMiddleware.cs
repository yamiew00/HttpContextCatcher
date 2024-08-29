using HttpContextCatcher.CatcherManager;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HttpContextCatcher
{
    public class HttpContextCatcherMiddleware
    {
        protected readonly RequestDelegate _Next;
        private readonly IAsyncCatcherService CatcherService;
        private readonly HttpContextCatcherOptionBuilder OptionBuilder;

        public HttpContextCatcherMiddleware(RequestDelegate next,
                                            IAsyncCatcherService catcherService,
                                            HttpContextCatcherOptionBuilder optionBuilder)
        {
            _Next = next;
            this.CatcherService = catcherService;
            this.OptionBuilder = optionBuilder;
        }

        public async Task Invoke(HttpContext context)
        {
            int startTick = Environment.TickCount;

            RequestCatcher requestCatcher = OptionBuilder.IsIgnoreRequest ?
                default :
                await CreateRequestCatcher(context);
            ResponseCatcher responseCatcher = default;
            ExceptionCatcher exceptionCatcher = default;
            DateTime now = DateTime.UtcNow;

            try
            {
                //判斷response有沒有抓取的必要
                if (!OptionBuilder.IsIgnoreResponse)
                {
                    Stream originalBody = context.Response.Body;
                    using var memStream = new MemoryStream();
                    context.Response.Body = memStream;

                    await _Next(context);

                    memStream.Position = 0;
                    string responseString = await new StreamReader(memStream).ReadToEndAsync();
                    responseCatcher = new ResponseCatcher(body: responseString, contentType: context.Response.ContentType);

                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                }
                else
                {
                    await _Next(context);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                ContextCatcher contextCatcher = new ContextCatcher(time: now,
                                                                   request: requestCatcher,
                                                                   response: responseCatcher,
                                                                   exception: exceptionCatcher,
                                                                   statusCode: context.Response.StatusCode,
                                                                   costSecond: (Environment.TickCount - startTick) / 1000M);

                await CatcherService.OnCatchAsync(contextCatcher);
            }
        }

        /// <summary>
        /// Get detailed content from HttpContext.Request
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<RequestCatcher> CreateRequestCatcher(HttpContext context)
        {
            try
            {
                var path = context.Request.Path;
                var method = context.Request.Method.ToString();
                var queries = context.GetRequestQueries();
                var headers = context.GetRequestHeaders();
                var body = await context.GetJsonRequestBody();

                return new RequestCatcher(path, method, body, queries, headers, context.Request.ContentType);
            }
            catch
            {
                return default;
            }
        }
    }
}
