using PowershellRunner.EventArgs;
using System;
using System.Collections.Generic;
using System.Management.Automation.Runspaces;

namespace PowershellRunner.Interfaces
{
    /// <summary>
    ///     Runs powershell code.
    /// </summary>
    public interface IPowershellRunner : IDisposable
    {
        /// <summary>
        ///     Occurs when [Error occurred].
        /// </summary>
        public event EventHandler<ErrorEventArgs>? OnError;

        /// <summary>
        ///     Occurs when [information occurred].
        /// </summary>
        event EventHandler<MessageEventArgs>? OnInformation;

        /// <summary>
        ///     Occurs when [progress occurred].
        /// </summary>
        event EventHandler<ProgressEventArgs>? OnProgress;

        /// <summary>
        ///     Occurs when [debug occurred].
        /// </summary>
        event EventHandler<MessageEventArgs>? OnDebug;

        /// <summary>
        ///     Occurs when [warning occurred].
        /// </summary>
        event EventHandler<MessageEventArgs>? OnWarning;

        /// <summary>
        ///     Occurs when [verbose occurred].
        /// </summary>
        event EventHandler<MessageEventArgs>? OnVerbose;

        /// <summary>
        ///     Invokes scripts from a file path.
        /// </summary>
        /// <param name="script">Script to be ran.</param>
        List<dynamic> Invoke(string script);

        List<dynamic> Invoke(string command, List<KeyValuePair<string, object?>> parameters);

        List<dynamic> Invoke(List<Command> commandList);
    }
}
