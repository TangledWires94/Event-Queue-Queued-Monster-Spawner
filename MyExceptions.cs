//Namespace to manage custom exception classes
namespace MyExceptions
{
    using System;
    public class QueueSizeException : Exception
    {
        string message;
        Exception inner;

        public QueueSizeException(string message = "Max queue size reached", Exception inner = null)
        {
            this.message = message;
            this.inner = inner;
        }
    }
}
