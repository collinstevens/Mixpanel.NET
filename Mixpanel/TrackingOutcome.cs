namespace Mixpanel
{
    public class TrackingOutcome
    {
        public TrackingOutcome(string status, string error)
        {
            Status = status;
            Error = error;
        }

        public bool Successful => Status.Equals("1");

        public string Status { get; }

        public string Error { get; }
    }
}
