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
            if(CatcherType != null && CatcherType != typeof(DoNothingCatcherService))
            {
                throw new Exception("Currently, only one catcher can be registered. Advanced features will be available in future versions.");
            }
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
