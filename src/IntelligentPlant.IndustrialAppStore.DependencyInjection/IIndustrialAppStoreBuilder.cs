using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection {

    /// <summary>
    /// A builder for configuring Industrial App Store services.
    /// </summary>
    public interface IIndustrialAppStoreBuilder {

        /// <summary>
        /// The service collection.
        /// </summary>
        IServiceCollection Services { get; }

    }

}
