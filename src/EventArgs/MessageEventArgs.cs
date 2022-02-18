using Serilog.Events;

namespace PowershellRunner.EventArgs
{
    public class MessageEventArgs : EventArgsBase
    {
        public LogEventLevel MessageLevel { get; }

        public MessageEventArgs(string message, LogEventLevel messageLevel) : base(message)
        {
            MessageLevel = messageLevel;
        }
    }
}
