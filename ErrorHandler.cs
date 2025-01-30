using System;
using System.Diagnostics;

namespace ACLSim
{
    public class ErrorHandler
    {
        public delegate void OnError(string message);
        public event OnError onError;

        public void DisplayError(string message)
        {
            if (onError != null)
            {
                Debug.WriteLine(DateTime.Now.ToLongTimeString() + " | *ERROR*: " + message + "\r\n");
                onError(DateTime.Now.ToLongTimeString() + " | *ERROR*: " + message + "\r\n");
            }
        }

        public void DisplayInfo(string message)
        {
            if (onError != null) { 
                onError(DateTime.Now.ToLongTimeString() + " | Info: " + message + "\r\n");
            }
        }
    }
}