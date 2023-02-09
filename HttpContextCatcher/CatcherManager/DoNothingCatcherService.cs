using System.Threading.Tasks;

namespace HttpContextCatcher.CatcherManager
{
    internal class DoNothingCatcherService : IAsyncCatcherService
    {
        public Task OnCatchAsync(ContextCatcher contextCatcher) => Task.CompletedTask;
    }
}
