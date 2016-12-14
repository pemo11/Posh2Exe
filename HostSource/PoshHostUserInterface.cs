// ====================================================================
// File: PoshHostUserInterface.cs
// This is a standard implementation of the PSHostUserInterface interface
// ====================================================================

namespace Posh2Exe
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Host;

    /// <summary>
    /// The bare minimum implementation of PSHostRawUserInterface
    /// </summary>
    public class PoshHostUserInterface : PSHostUserInterface
    {
        /// <summary>
        /// An instance of the PSRawUserInterface object.
        /// </summary>
        private PoshHostRawUserInterface poshRawUI = new PoshHostRawUserInterface();

        /// <summary>
        /// Gets an instance of the PSRawUserInterface object for this host
        /// application.
        /// </summary>
        public override PSHostRawUserInterface RawUI
        {
            get { return this.poshRawUI; }
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            throw new NotImplementedException("Not implemented because not needed.");
        }

        public override int PromptForChoice(string caption, string message,Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException("Not implemented because not needed.");
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException("Not implemented because not needed.");
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotImplementedException("Not implemented because not needed.");
        }

        public override string ReadLine()
        {
            return Console.ReadLine();
        }

        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException("Not implemented because not needed.");
        }

        public override void Write(string value)
        {
            System.Console.Write(value);
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            System.Console.Write(value);
        }

        public override void WriteDebugLine(string message)
        {
            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "DEBUG: {0}", message));
        }

        public override void WriteErrorLine(string value)
        {
            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "ERROR: {0}", value));
        }

        public override void WriteLine()
        {
            System.Console.WriteLine();
        }

        public override void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }

        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            System.Console.WriteLine(value);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            // Hier passiert nichts
        }

        public override void WriteVerboseLine(string message)
        {
            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "VERBOSE: {0}", message));
        }

        public override void WriteWarningLine(string message)
        {
            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "WARNING: {0}", message));
        }
    }
}

