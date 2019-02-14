using System.Threading;
using System.Threading.Tasks;

namespace Mixpanel
{
    public interface IClient
    {
        Event CreateEvent();

        Task<bool> TrackAsync(Event @event);

        Task<bool> TrackAsync(Event @event, CancellationToken cancellationToken);
    }
}
