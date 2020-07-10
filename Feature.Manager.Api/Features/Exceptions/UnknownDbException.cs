using System;

namespace Feature.Manager.Api.Features.Exceptions
{
    public class UnknownDbException : Exception
    {
        public UnknownDbException(string message) : base(message)
        {
        }
    }
}
