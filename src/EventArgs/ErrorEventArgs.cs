using System;
using System.Collections.Generic;

namespace PowershellRunner.EventArgs
{
    public class ErrorEventArgs : EventArgsBase
    {
        public Exception? Exception { get; }

        public ErrorEventArgs(string message) : base(message)
        {
            Exception = null;
        }

        public ErrorEventArgs(List<string> messages) : this(string.Join(Environment.NewLine, messages ?? new List<string>()))
        {
        }

        public ErrorEventArgs(Exception ex) : this(ex?.Message ?? "")
        {
            Exception = ex;
        }

        public string GetErrorMessage() => Exception?.Message ?? Message ?? string.Empty;
    }
}
