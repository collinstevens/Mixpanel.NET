using System.Threading;
using System.Threading.Tasks;

namespace Mixpanel
{
    public interface IMixpanelClient
    {
        MixpanelEvent CreateEvent();

        Task<TrackingOutcome> TrackAsync(MixpanelEvent @event);

        Task<TrackingOutcome> TrackAsync(MixpanelEvent @event, CancellationToken cancellationToken);
    }
}
