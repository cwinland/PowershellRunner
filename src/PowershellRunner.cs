using PowershellRunner.EventArgs;
using PowershellRunner.Interfaces;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PowershellRunner
{
    /// <inheritdoc />
    public class PowerShellRunner : IPowershellRunner
    {
        public Command? PreScript { get; set; }
        public bool AddMachinePaths { get; set; }
        private const string POWER_SHELL_PRE_SCRIPT = "$env:PSModulePath = [System.Environment]::GetEnvironmentVariable('PSModulePath','Machine'); ";

        private bool disposedValue;

        protected PowerShell PowerShell { get; }

        public PowerShellRunner(int majorVersion = 5, int minorVersion = 1)
        {
            PowerShell = GetPowerShellInstance(majorVersion, minorVersion);
            SetupStreams();
        }

        public PowerShellRunner(PowerShell powerShellInstance)
        {
            PowerShell = powerShellInstance;
            SetupStreams();
        }

        /// <inheritdoc />
        public List<dynamic> Invoke(string script)
        {
            PowerShell.AddScript($"{POWER_SHELL_PRE_SCRIPT}; {script}");
            List<dynamic>? result = PowerShell?.Invoke()?.Select(x => x as dynamic).ToList();
            ReportPsErrors(PowerShell);
            return result ?? new List<dynamic>();
        }

        /// <inheritdoc />
        public List<dynamic> Invoke(string command, List<KeyValuePair<string, object?>> parameters)
        {
            var invokeCommand = CreateCommand(command, parameters);

            List<Command> commandList = new()
            {
                new Command(POWER_SHELL_PRE_SCRIPT, true),
                invokeCommand
            };

            return Invoke(commandList) ?? new List<dynamic>();
        }

        /// <inheritdoc />
        public List<dynamic> Invoke(List<Command> commandList)
        {
            Pipeline pipeline;
            List<dynamic> results = new();
            commandList.ForEach(cmd =>
            {
                pipeline = PowerShell.Runspace.CreatePipeline();
                pipeline.Commands.Add(cmd);
                results.Add(pipeline.Invoke());
            });

            ReportPsErrors(PowerShell);
            return results ?? new List<dynamic>();
        }

        /// <inheritdoc />
        public event EventHandler<MessageEventArgs>? OnDebug;

        /// <inheritdoc />
        public event EventHandler<ErrorEventArgs>? OnError;

        /// <inheritdoc />
        public event EventHandler<MessageEventArgs>? OnInformation;

        /// <inheritdoc />
        public event EventHandler<ProgressEventArgs>? OnProgress;

        /// <inheritdoc />
        public event EventHandler<MessageEventArgs>? OnVerbose;

        /// <inheritdoc />
        public event EventHandler<MessageEventArgs>? OnWarning;

        public static Command CreateCommand(string command, List<KeyValuePair<string, object?>> parameters)
        {
            var invokeCommand = new Command(command, false);
            parameters.ForEach(x => invokeCommand.Parameters.Add(x.Value != null
                ? new CommandParameter(x.Key, x.Value)
                : new CommandParameter(x.Key)));
            return invokeCommand;
        }

        public static Command CreateScript(string script)
        {
            return new Command(script, true);
        }

        internal PowerShell GetPowerShellInstance(int majorVersion, int minorVersion)
        {
            var powerShellProcess =
                new PowerShellProcessInstance(new Version(majorVersion, minorVersion), null, null, false);
            powerShellProcess.Process.Start();
            var table = new TypeTable(new List<string>());
            var runSpace = RunspaceFactory.CreateOutOfProcessRunspace(table, powerShellProcess);
            runSpace.Open();
            return PowerShell.Create(runSpace);
        }

        internal void ReportPsErrors(PowerShell? ps)
        {
            if (ps == null) return;

            if (ps.HadErrors && OnError == null)
                ps.Streams?.Error?.ReadAll()?.ToList()
                    .ForEach(x => throw x?.Exception ?? new Exception("Unknown Exception"));
        }

        private void SetupStreams()
        {
            PowerShell.Streams.Error.DataAdded += (sender, args) =>
                OnError?.Invoke(sender, new ErrorEventArgs(PowerShell.Streams.Error[args.Index].Exception));
            PowerShell.Streams.Information.DataAdded += (sender, args) => OnInformation?.Invoke(sender,
                new MessageEventArgs(PowerShell.Streams.Information[args.Index]?.MessageData.ToString(),
                    LogEventLevel.Information));
            PowerShell.Streams.Debug.DataAdded += (sender, args) => OnDebug?.Invoke(sender,
                new MessageEventArgs(PowerShell.Streams.Debug[args.Index]?.Message, LogEventLevel.Debug));
            PowerShell.Streams.Warning.DataAdded += (sender, args) => OnWarning?.Invoke(sender,
                new MessageEventArgs(PowerShell.Streams.Warning[args.Index]?.Message, LogEventLevel.Warning));
            PowerShell.Streams.Verbose.DataAdded += (sender, args) => OnVerbose?.Invoke(sender,
                new MessageEventArgs(PowerShell.Streams.Verbose[args.Index]?.Message, LogEventLevel.Verbose));

            PowerShell.Streams.Progress.DataAdded += (sender, args) =>
            {
                var progressRecord = PowerShell.Streams.Progress[args.Index];
                OnProgress?.Invoke(sender,
                    new ProgressEventArgs(progressRecord?.Activity, progressRecord.PercentComplete,
                        progressRecord.SecondsRemaining, progressRecord.StatusDescription));
            };
        }

        #region IDispose

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;

            if (disposing) PowerShell.Dispose();

            disposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}