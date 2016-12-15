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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="descriptions"></param>
        /// <returns></returns>
        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            this.Write(
                ConsoleColor.Blue,
                ConsoleColor.Black,
                caption + "\n" + message + " ");
                Dictionary<string, PSObject> results = new Dictionary<string, PSObject>();
                foreach (FieldDescription fd in descriptions)
                {
                    string[] label = GetHotkeyAndLabel(fd.Label);
                    this.WriteLine(label[1]);
                    string userData = Console.ReadLine();
                    if (userData == null)
                    {
                        return null;
                    }
                    results[fd.Name] = PSObject.AsPSObject(userData);
                }
            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="choices"></param>
        /// <param name="defaultChoice"></param>
        /// <returns></returns>
        public override int PromptForChoice(string caption, string message,Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException("PromptForChoice - not implemented because not needed.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="userName"></param>
        /// <param name="targetName"></param>
        /// <returns></returns>
        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException("PromptForCredential - not implemented yet.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="userName"></param>
        /// <param name="targetName"></param>
        /// <param name="allowedCredentialTypes"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotImplementedException("PromptForCredential - not implemented yet.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ReadLine()
        {
            return Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException("ReadLineAsSecureString - not implemented yet.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void Write(string value)
        {
            System.Console.Write(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="value"></param>
        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            ConsoleColor oldForeColor = Console.ForegroundColor;
            ConsoleColor oldBackColor = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(value);
            Console.ForegroundColor = oldForeColor;
            Console.BackgroundColor = oldBackColor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void WriteDebugLine(string message)
        {
            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "DEBUG: {0}", message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void WriteErrorLine(string value)
        {
            this.WriteLine(
                ConsoleColor.Red,
                ConsoleColor.Black, 
                String.Format(CultureInfo.CurrentCulture, "ERROR: {0}", value));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void WriteLine()
        {
            System.Console.WriteLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="value"></param>
        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            ConsoleColor oldForeColor = Console.ForegroundColor;
            ConsoleColor oldBackColor = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(value);
            Console.ForegroundColor = oldForeColor;
            Console.BackgroundColor = oldBackColor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="record"></param>
        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            // Hier passiert nichts
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void WriteVerboseLine(string message)
        {
            this.WriteLine(
                ConsoleColor.Green,
                ConsoleColor.Black, 
                String.Format(CultureInfo.CurrentCulture, "VERBOSE: {0}", message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void WriteWarningLine(string message)
        {
            this.WriteLine(
                ConsoleColor.Yellow,
                ConsoleColor.Black,
                String.Format(CultureInfo.CurrentCulture, "WARNING: {0}", message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string[] GetHotkeyAndLabel(string input)
        {
            string[] result = new string[] { String.Empty, String.Empty };
            string[] fragments = input.Split('&');
            if (fragments.Length == 2)
            {
                if (fragments[1].Length > 0)
                {
                    result[0] = fragments[1][0].ToString().
                    ToUpper(CultureInfo.CurrentCulture);
                }

                result[1] = (fragments[0] + fragments[1]).Trim();
            }
            else
            {
                result[1] = input;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="choices"></param>
        /// <returns></returns>
        private static string[,] BuildHotkeysAndPlainLabels(
            Collection<ChoiceDescription> choices)
        {
            // we will allocate the result array
            string[,] hotkeysAndPlainLabels = new string[2, choices.Count];

            for (int i = 0; i < choices.Count; ++i)
            {
                string[] hotkeyAndLabel = GetHotkeyAndLabel(choices[i].Label);
                hotkeysAndPlainLabels[0, i] = hotkeyAndLabel[0];
                hotkeysAndPlainLabels[1, i] = hotkeyAndLabel[1];
            }

            return hotkeysAndPlainLabels;
        }
    }
}

