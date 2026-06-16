using static Microsoft.Extensions.Options.Options;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Tests;

public class TokenStoreExtensionsTests {

    private static async Task<TestTokenStore> InitializedStore(OAuthTokens? token = null) {
        var options = Create(new IndustrialAppStoreAuthenticationOptions { ClientId = "test-client" });
        var store = new TestTokenStore(options, new HttpClient(), TimeProvider.System);
        if (token.HasValue) {
            store.SetTokens(token.Value);
        }
        await ((ITokenStore)store).InitAsync("user1", "session1");
        return store;
    }

    [Fact]
    public async Task GetAuthenticationHeaderAsync_ThrowsForNullStore() {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => ((ITokenStore)null!).GetAuthenticationHeaderAsync());
    }

    [Fact]
    public async Task GetAuthenticationHeaderAsync_ReturnsNull_WhenNoTokenStored() {
        var store = await InitializedStore();
        Assert.Null(await store.GetAuthenticationHeaderAsync());
    }

    [Fact]
    public async Task GetAuthenticationHeaderAsync_ReturnsBearerHeader() {
        var store = await InitializedStore(new OAuthTokens("Bearer", "my-access-token", null, null));

        var header = await store.GetAuthenticationHeaderAsync();

        Assert.NotNull(header);
        Assert.Equal("Bearer", header!.Scheme);
        Assert.Equal("my-access-token", header.Parameter);
    }

    [Fact]
    public async Task GetAuthenticationHeaderAsync_AlwaysUsesBearerScheme_RegardlessOfTokenType() {
        // The implementation hardcodes "Bearer" and does not forward the TokenType from OAuthTokens.
        var store = await InitializedStore(new OAuthTokens("mac", "my-access-token", null, null));

        var header = await store.GetAuthenticationHeaderAsync();

        Assert.NotNull(header);
        Assert.Equal("Bearer", header!.Scheme);
    }

}
