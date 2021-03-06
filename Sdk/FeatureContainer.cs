using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Fossapps.FeatureManager.Models;

namespace Fossapps.FeatureManager
{
    // this is singleton
    public interface IFeatureWorker
    {
        IEnumerable<RunningFeature> GetRunningFeatures();
        void Init();
    }
    public class FeatureWorker : IFeatureWorker
    {
        private readonly TimeSpan _syncInterval;
        private readonly FeatureManager _manager;
        private IEnumerable<RunningFeature> _features = new List<RunningFeature>();

        public FeatureWorker(string endpoint, TimeSpan syncInterval)
        {
            _syncInterval = syncInterval;
            _manager = new FeatureManager(new Uri(endpoint));
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

    // this is scoped
    public class FeatureClient
    {
        private readonly IFeatureWorker _featureWorker;
        private readonly IUserDataRepo _userDataRepo;

        public FeatureClient(IFeatureWorker featureWorker, IUserDataRepo userDataRepo)
        {
            _featureWorker = featureWorker;
            _userDataRepo = userDataRepo;
        }

        private RunningFeature GetFeatureById(string featId)
        {
            return _featureWorker.GetRunningFeatures().FirstOrDefault(x => x.FeatureId == featId);
        }
        public char GetVariant(string featId)
        {
            if (_userDataRepo.GetForcedFeaturesB().Any(x => x == featId))
            {
                return 'B';
            }
            if (_userDataRepo.GetForcedFeaturesA().Any(x => x == featId))
            {
                return 'A';
            }
            var feature = GetFeatureById(featId);

            if (feature == null || !feature.Allocation.HasValue || string.IsNullOrEmpty(feature.RunToken) || string.IsNullOrEmpty(feature.FeatureToken))
            {
                return 'X';
            }

            if (feature.RunStatus == "AllB")
            {
                return 'B';
            }

            if (feature.Allocation.Value == 100 || GetBucket(_userDataRepo.GetUserId(), feature.FeatureToken, 100) <= feature.Allocation)
            {
                var bucket = GetBucket(_userDataRepo.GetUserId(), feature.RunToken, 2);
                return bucket switch
                {
                    1 => 'A',
                    2 => 'B',
                    _ => 'X'
                };
            }
            return 'Z';
        }

        private static int GetBucket(string userToken, string bucketToken, int numberOfBuckets)
        {
            var tokens = (userToken.Trim() + bucketToken.Trim()).Replace("-", "").ToLower();
            using var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(tokens));
            var number = BitConverter.ToUInt64(bytes);
            return (int) (number % (ulong) numberOfBuckets) + 1;
        }
    }
}
