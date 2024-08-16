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
            DateTime now = DateTime.Now; // 獲取當前時間

            // 
            Stream originalBody = context.Response.Body; // 儲存原始的 response.Body
            using var memStream = new MemoryStream();
            context.Response.Body = memStream; // 將 response.Body 替換為記憶體流

            try
            {
                // 執行下一步中介層邏輯
                await _Next(context);

                // 如果設定忽略 response，就直接將內容寫回原始 Body
                if (OptionBuilder.IsIgnoreResponse)
                {
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody); // 將內容寫回原始的 response.Body
                    context.Response.Body = originalBody; // 還原 response.Body
                    return;
                }

                // 獲取 response 的內容
                memStream.Position = 0;
                string responseString = await new StreamReader(memStream).ReadToEndAsync();
                Console.WriteLine($"Response captured: {responseString}"); // 診斷輸出

                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody); // 將內容寫回到原始的 response.Body

                responseCatcher = new ResponseCatcher(statusCode: context.Response.StatusCode,
                                                      body: responseString,
                                                      contentType: context.Response.ContentType);
            }
            catch (Exception ex)
            {
                // 處理特定的 BSON 轉換錯誤
                if (ex is InvalidCastException &&
                   ex.Message == "Unable to cast object of type 'MongoDB.Bson.BsonArray' to type 'MongoDB.Bson.BsonBoolean'.")
                {
                    var errorMessage =
        @"Please add the .AddBsonSerializer() method to the IMVCBuilder to parse BSON formatted data.
Example:
    builder.Services.AddControllers()
                .AddBsonSerializer();";
                    ex = new InvalidCastException(errorMessage);
                }

                // 如果忽略 response，則直接還原原始 response.Body 並拋出異常
                if (OptionBuilder.IsIgnoreResponse)
                {
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody); // 確保寫回原始的 Body
                    context.Response.Body = originalBody;
                    throw;
                }

                string responseBody = default;
                int statusCode = default;

                // 判斷 response 是否已有內容
                if (ResponseHasContent(context))
                {
                    // 擷取可能已被修改的 response 內容
                    memStream.Position = 0;
                    responseBody = await new StreamReader(memStream).ReadToEndAsync();
                    Console.WriteLine($"Exception response captured: {responseBody}"); // 診斷輸出

                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);

                    statusCode = context.Response.StatusCode;
                }
                else
                {
                    // response 沒有被修改過，直接拋出異常
                    responseBody = $"{ex.Message}{ex.StackTrace}";
                    statusCode = StatusCodes.Status500InternalServerError;
                }

                responseCatcher = new ResponseCatcher(statusCode: context.Response.StatusCode,
                                                      body: responseBody,
                                                      contentType: context.Response.ContentType);

                context.Response.Body = originalBody;   // 還原 response.Body

                // 記錄異常
                exceptionCatcher = new ExceptionCatcher(ex);

                throw; // 將異常重新拋出
            }
            finally
            {
                ContextCatcher contextCatcher = new ContextCatcher(now,
                                                                   requestCatcher,
                                                                   responseCatcher,
                                                                   exceptionCatcher);

                // 計算響應所花的時間
                contextCatcher.CostSecond = (Environment.TickCount - startTick) / 1000M;

                // 處理 contextCatcher
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
