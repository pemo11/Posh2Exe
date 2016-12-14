// ====================================================================
// File: Posh2ExeException.cs
// An exeception class fore catching more specific errors
// ====================================================================

namespace Posh2Exe
{
    using System;

    /// <summary>
    /// Just a custom exception class for the application
    /// </summary>
    public class Posh2ExeException : SystemException
    {
        // Constructor with the message string as an argument
        public Posh2ExeException(string msg) : base(msg)
        {

        }
    }
}
