using System;

namespace Feature.Manager.Api.Features.Exceptions
{
    public class FeatureCreatingFailedException : Exception
    {
        public FeatureCreatingFailedException(string message) : base(message)
        {
        }
    }
}
