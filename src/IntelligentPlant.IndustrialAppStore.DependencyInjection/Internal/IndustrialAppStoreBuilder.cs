using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection.Internal {

    internal class IndustrialAppStoreBuilder : IIndustrialAppStoreBuilder {

        public IServiceCollection Services { get; }


        public IndustrialAppStoreBuilder(IServiceCollection services) {
            Services = services;
        }

    }

}
