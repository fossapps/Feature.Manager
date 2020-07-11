using System;
using Microsoft.Extensions.DependencyInjection;

namespace Fossapps.FeatureManager
{
    public static class SetupClient
    {
        public static void SetupFeatureClients<TUserDataImplementation>(this IServiceCollection collection, string endpoint, TimeSpan syncInterval) where TUserDataImplementation : class, IUserDataRepo
        {
            var worker = new FeatureWorker(endpoint, syncInterval);
            worker.Init();
            collection.AddScoped<IUserDataRepo, TUserDataImplementation>();
            collection.AddSingleton<IFeatureWorker>(worker);
            collection.AddScoped<FeatureClient>();
        }

        public static bool IsFeatureOn(this FeatureClient instance, string featId)
        {
            return instance.GetVariant(featId) == 'B';
        }
    }
}
