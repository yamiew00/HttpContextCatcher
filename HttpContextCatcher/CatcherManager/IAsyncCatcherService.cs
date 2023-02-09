using System.Threading.Tasks;

namespace HttpContextCatcher.CatcherManager
{
    public interface IAsyncCatcherService
    {
        public Task OnCatchAsync(ContextCatcher contextCatcher);
    }
}
