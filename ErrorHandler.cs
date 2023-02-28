using System;

namespace ACLSim
{
    public class ErrorHandler
    {
        public delegate void OnError(string message);
        public event OnError onError;

        public void DisplayError(string message)
        {
            onError(DateTime.Now.ToLongTimeString() + " | *ERROR*: " + message + "\r\n");
        }

        public void DisplayInfo(string message)
        {
            onError(DateTime.Now.ToLongTimeString() + " | Info: " + message + "\r\n");
        }
    }
}