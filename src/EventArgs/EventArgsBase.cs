namespace PowershellRunner.EventArgs
{
    public abstract class EventArgsBase : System.EventArgs
    {
        public string Message { get; }

        protected EventArgsBase(string message = "")
        {
            Message = message ?? string.Empty;
        }
    }
}
