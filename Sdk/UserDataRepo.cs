using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Fossapps.FeatureManager
{
    public interface IUserDataRepo
    {
        public string GetUserId();

        public IEnumerable<string> GetForcedFeaturesA()
        {
            return new List<string>();
        }

        public IEnumerable<string> GetForcedFeaturesB()
        {
            return new List<string>();
        }
    }
    public abstract class UserDataRepoBase : IUserDataRepo
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public abstract string GetUserId();

        protected UserDataRepoBase(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public IEnumerable<string> GetForcedFeaturesA()
        {
            if (!_contextAccessor.HttpContext.Request.Headers.TryGetValue("X-Forced-Features-A", out var features))
            {
                return new List<string>();
            }

            return features.ToString().Split(",").ToList();
        }

        public IEnumerable<string> GetForcedFeaturesB()
        {
            if (!_contextAccessor.HttpContext.Request.Headers.TryGetValue("X-Forced-Features-B", out var features))
            {
                return new List<string>();
            }

            return features.ToString().Split(",").ToList();
        }
    }
}
