// ====================================================================
// File: PoshHost.cs
// A standard implementation of the PSHost class with the bare
// minimum of the required functionality
// ====================================================================

namespace Posh2Exe
{
    using System;
    using System.Globalization;
    using System.Management.Automation.Host;
    using System.Threading;

    public class PoshHost : PSHost
    {
        /// <summary>
        /// Reference to the Host Application
        /// </summary>
        private PoshHostApplication hostApp;

        private CultureInfo originalCultureInfo = Thread.CurrentThread.CurrentCulture;

        private CultureInfo originalUICultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture;

        private Guid hostId = Guid.NewGuid();

        public PoshHost(PoshHostApplication HostApp)
        {
            this.hostApp = HostApp;
        }

        public override System.Globalization.CultureInfo CurrentCulture
        {
            get { return this.originalCultureInfo; }
        }

        public override System.Globalization.CultureInfo CurrentUICulture
        {
            get { return this.originalUICultureInfo; }
        }

        public override Guid InstanceId
        {
            get { return this.hostId; }
        }

        public override string Name
        {
            get { return "PoshHostV2"; }
        }

        public override PSHostUserInterface UI
        {
            get { return this.poshUserInterface; }
        }

        public override Version Version
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        private PoshHostUserInterface poshUserInterface = new PoshHostUserInterface();

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException("Not implemented because not needed");
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException("Not implemented because not needed");
        }

        public override void NotifyBeginApplication()
        {
            return;  // Nothing is ever happening here
        }

        public override void NotifyEndApplication()
        {
            return;  // Nothing is ever happening here
        }

        public override void SetShouldExit(int exitCode)
        {
            this.hostApp.ShouldExit = true;
            this.hostApp.ExitCode = exitCode;
        }
    }
}

