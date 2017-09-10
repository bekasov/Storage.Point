namespace StoragePoint.Domain.Exceptions
{
    using System;

    public abstract class BaseException : Exception
    {
        protected BaseException()
        {
        }

        protected BaseException(string message) : base(message)
        {
        }
    }
}