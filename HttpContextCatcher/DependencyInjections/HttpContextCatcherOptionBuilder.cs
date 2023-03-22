using HttpContextCatcher.CatcherManager;
using System;

namespace HttpContextCatcher
{
    public class HttpContextCatcherOptionBuilder
    {
        internal Type CatcherType { get; private set; }

        internal bool IsIgnoreRequest { get; private set; }

        internal bool IsIgnoreResponse { get; private set; }

        internal HttpContextCatcherOptionBuilder()
        {
            SetCatcher<DoNothingCatcherService>();
            IsIgnoreRequest = false;
            IsIgnoreResponse = false;
        }

        public void SetCatcher<T>() where T : IAsyncCatcherService
        {
            CatcherType = typeof(T);
        }

        /// <summary>
        /// httpContextCatcher will ignore all request
        /// </summary>
        public void IgnoreRequest()
        {
            IsIgnoreRequest = true;
        }

        /// <summary>
        /// httpContextCatcher will ignore all response
        /// </summary>
        public void IgnoreResponse()
        {
            IsIgnoreResponse = true;
        }
    }
}
