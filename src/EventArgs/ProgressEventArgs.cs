namespace PowershellRunner.EventArgs
{
    public class ProgressEventArgs : EventArgsBase
    {
        public string Activity { get; }

        public int PercentComplete { get; }

        public int SecondsRemaining { get; }

        public string StatusDescription { get; }
        public ProgressEventArgs(string activity, int percentComplete, int secondsRemaining, string statusDescription) :base("")
        {
            Activity = activity;
            PercentComplete = percentComplete;
            SecondsRemaining = secondsRemaining;
            StatusDescription = statusDescription;
        }
    }
}
