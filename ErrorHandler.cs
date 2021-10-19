using System;

namespace ACLSim
{
    public class ErrorHandler
    {
        public delegate void OnError(string message);
        public event OnError onError;

        public void DisplayError(string message)
        {
            onError(DateTime.Now.ToLongTimeString() + " | Error: " + message + "\r\n");
        }
    }
}