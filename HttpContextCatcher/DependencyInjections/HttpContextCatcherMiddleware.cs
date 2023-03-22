using HttpContextCatcher.CatcherManager;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
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
            var startTick = Environment.TickCount;
            RequestCatcher requestCatcher = OptionBuilder.IsIgnoreRequest ? 
                default : 
                await CreateRequestCatcher(context);
            ResponseCatcher responseCatcher = default;
            ExceptionCatcher exceptionCatcher = default;
            DateTime now = DateTime.Now; //時間要靠傳入的

            //response part1
            Stream originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            //執行下一步 (順序很重要)
            try
            {
                await _Next(context);

                if(OptionBuilder.IsIgnoreResponse) 
                {
                    context.Response.Body = originalBody;
                    return;
                }

                //擷取人為修改的response
                memStream.Position = 0;
                string responseString = new StreamReader(memStream).ReadToEnd();

                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);

                responseCatcher = new ResponseCatcher(statusCode: context.Response.StatusCode,
                                                      body: responseString);
            }
            catch (Exception ex)
            {
                if (OptionBuilder.IsIgnoreResponse)
                {
                    context.Response.Body = originalBody;
                    throw ex;
                }

                string responseBody = default;
                int statusCode = default;

                //有可能response已被其他middleware修改過
                if (ResponseHasContent(context))
                {
                    //擷取人為修改的response
                    memStream.Position = 0;
                    responseBody = new StreamReader(memStream).ReadToEnd();

                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);

                    statusCode = context.Response.StatusCode;
                }
                else
                {
                    //沒被修改過，直接拋exception到最外層 → 會顯示ex.Message 且 500 statusCode 
                    responseBody = $"{ex.Message}{ex.StackTrace}";
                    statusCode = StatusCodes.Status500InternalServerError;
                }

                responseCatcher = new ResponseCatcher(statusCode: context.Response.StatusCode,
                                                      body: responseBody);

                context.Response.Body = originalBody;   //將response.body復原

                //記下exception
                exceptionCatcher = new ExceptionCatcher(ex);

                throw ex; //抓到pipeline中的exception也要原封不動拋出去。
            }
            finally
            {
                ContextCatcher contextCatcher = new ContextCatcher(now, 
                                                                   requestCatcher,
                                                                   responseCatcher,
                                                                   exceptionCatcher);
                //timing ends
                if(!OptionBuilder.IsIgnoreResponse) contextCatcher.SetResSecond((Environment.TickCount - startTick) / 1000M);

                //deal with contextCatcher by option
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

                return new RequestCatcher(path, method, body, queries, headers);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Return true if httpContext.Response has been modified by other middleware
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool ResponseHasContent(HttpContext context)
        {
            return context.Response.Body.Length > 0;
        }
    }
}
