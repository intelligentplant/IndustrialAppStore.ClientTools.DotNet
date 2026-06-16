using Microsoft.Extensions.DependencyInjection;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Tests;

public class ResilienceConfigurationTests {

    [Fact]
    public void ResiliencePipeline_CanBeBuilt_WithoutErrors() {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddIndustrialAppStoreAuthentication(options => {
            options.ClientId = "test-client";
        });

        using var provider = services.BuildServiceProvider();

        // CreateHandler is where the resilience pipeline is constructed and validated.
        var handlerFactory = provider.GetRequiredService<IHttpMessageHandlerFactory>();

        var ex = Record.Exception(() => handlerFactory.CreateHandler("IndustrialAppStoreHttpClient"));

        Assert.Null(ex);
    }

}
