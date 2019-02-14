using System.Threading;
using System.Threading.Tasks;

namespace Mixpanel
{
    public interface IClient
    {
        Event CreateEvent();

        Task<TrackingOutcome> TrackAsync(Event @event);

        Task<TrackingOutcome> TrackAsync(Event @event, CancellationToken cancellationToken);

        Task<TrackingOutcome> TrackAsync(Event @event, bool verbose);

        Task<TrackingOutcome> TrackAsync(Event @event, bool verbose, CancellationToken cancellationToken);
    }
}
