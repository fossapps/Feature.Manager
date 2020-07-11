using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FossApps.FeatureManager;
using FossApps.FeatureManager.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Fossapps.FeatureManager
{
    // this is singleton
    public class FeatureWorker
    {
        private readonly TimeSpan _syncInterval;
        private readonly FossApps.FeatureManager.FeatureManager _manager;
        private IEnumerable<RunningFeature> _features;

        public FeatureWorker(string endpoint, TimeSpan syncInterval)
        {
            _syncInterval = syncInterval;
            _manager = new FossApps.FeatureManager.FeatureManager(new Uri(endpoint));
        }

        public IEnumerable<RunningFeature> GetRunningFeatures()
        {
            return _features;
        }

        public async void Init()
        {
            while (true)
            {
                try
                {
                    _features = await GetFeatures();
                }
                catch (Exception)
                {
                    // do nothing for now,
                }
                await Task.Delay(_syncInterval);
            }
        }

        private async Task<List<RunningFeature>> GetFeatures()
        {
            var result = await _manager.FeatureRuns.GetRunningFeaturesAsync();
            return result switch
            {
                IEnumerable<RunningFeature> list => list.ToList(),
                _ => throw new Exception()
            };
        }
    }

    public interface IUserDataRepo
    {
        public string GetUserId();
    }

    // this is scoped
    public class FeatureClient
    {
        private readonly FeatureWorker _featureWorker;
        private readonly IUserDataRepo _userDataRepo;

        public FeatureClient(FeatureWorker featureWorker, IUserDataRepo userDataRepo)
        {
            _featureWorker = featureWorker;
            _userDataRepo = userDataRepo;
        }

        public bool IsFeatureOn(string featId)
        {
            var feature = _featureWorker.GetRunningFeatures().FirstOrDefault(x => x.FeatureId == featId);
            if (feature == null)
            {
                return false;
            }

            return true;
        }
    }

    public static class SetupFeatures
    {
        public static void SetupFeatureClients<TUserDataImplementation>(this IServiceCollection collection, string endpoint, TimeSpan syncInterval) where TUserDataImplementation : class, IUserDataRepo
        {
            var worker = new FeatureWorker(endpoint, syncInterval);
            worker.Init();
            collection.AddScoped<IUserDataRepo, TUserDataImplementation>();
            collection.AddSingleton(worker);
            collection.AddScoped<FeatureClient>();
        }
    }
}
