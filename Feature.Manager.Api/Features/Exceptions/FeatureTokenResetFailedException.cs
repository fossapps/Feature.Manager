using System;

namespace Feature.Manager.Api.Features.Exceptions
{
    public class FeatureTokenResetFailedException : Exception
    {
        public FeatureTokenResetFailedException(string message) : base(message)
        {
        }
    }
}