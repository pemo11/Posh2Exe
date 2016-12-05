// File: Posh2ExeException.cs

namespace Posh2Exe
{
    using System;

    public class Posh2ExeException : SystemException
    {
        // Constructor with the message string as an argument
        public Posh2ExeException(string msg) : base(msg)
        {

        }
    }
}
