namespace Mixpanel
{
    public class TrackingOutcome
    {
        public TrackingOutcome(bool isVerbose, string status, string error)
        {
            IsVerbose = isVerbose;
            Status = status;
            Error = error;
        }

        public bool Successful => Status.Equals("1");

        public bool IsVerbose { get; }

        public string Status { get; }

        public string Error { get; }
    }
}
