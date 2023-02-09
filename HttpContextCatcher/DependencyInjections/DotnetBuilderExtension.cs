using HttpContextCatcher.CatcherManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HttpContextCatcher
{
    public static class DotnetBuilderExtension
    {
        /// <summary>
        /// Add an catcher to read the request and response of your Restful API
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpContextCatcher(this IServiceCollection builder, Action<HttpContextCatcherOptionBuilder> builderAction)
        {
            HttpContextCatcherOptionBuilder catcherBuilder = new HttpContextCatcherOptionBuilder();
            builderAction(catcherBuilder);
            builder.AddSingleton(catcherBuilder);

            builder.AddSingleton(serviceType: typeof(IAsyncCatcherService),
                                 implementationType: catcherBuilder.CatcherType);
            return builder;
        }

        public static void UseHttpContextCatcher(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<HttpContextCatcherMiddleware>();
        }
    }
}
