using static Microsoft.Extensions.Options.Options;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Tests;

public class TokenStoreTests {

    private static TestTokenStore CreateStore(TimeProvider? timeProvider = null) {
        var options = Create(new IndustrialAppStoreAuthenticationOptions { ClientId = "test-client" });
        return new TestTokenStore(options, new HttpClient(), timeProvider ?? TimeProvider.System);
    }

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    [Fact]
    public void Ready_IsFalse_BeforeInit() {
        Assert.False(CreateStore().Ready);
    }

    [Fact]
    public async Task Ready_IsTrue_AfterInit() {
        var store = CreateStore();
        await ((ITokenStore)store).InitAsync("user1", "session1");
        Assert.True(store.Ready);
    }

    [Fact]
    public async Task InitAsync_ThrowsForNullUserId() {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => ((ITokenStore)CreateStore()).InitAsync(null!, "session1").AsTask());
    }

    [Fact]
    public async Task InitAsync_ThrowsForNullSessionId() {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => ((ITokenStore)CreateStore()).InitAsync("user1", null!).AsTask());
    }

    [Fact]
    public async Task InitAsync_ThrowsOnSecondCall() {
        var store = CreateStore();
        await ((ITokenStore)store).InitAsync("user1", "session1");
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ((ITokenStore)store).InitAsync("user1", "session1").AsTask());
    }

    [Fact]
    public async Task GetTokensAsync_ThrowsIfNotInitialized() {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ((ITokenStore)CreateStore()).GetTokensAsync().AsTask());
    }

    [Fact]
    public async Task SaveTokensAsync_ThrowsIfNotInitialized() {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => ((ITokenStore)CreateStore()).SaveTokensAsync(new OAuthTokens("Bearer", "t", null, null)).AsTask());
    }

    // -------------------------------------------------------------------------
    // Token retrieval — expiry logic
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetTokensAsync_ReturnsNull_WhenNoTokenStored() {
        var store = CreateStore();
        await ((ITokenStore)store).InitAsync("user1", "session1");
        Assert.Null(await ((ITokenStore)store).GetTokensAsync());
    }

    [Fact]
    public async Task GetTokensAsync_ReturnsNull_WhenAccessTokenIsWhitespace() {
        // OAuthTokens ctor only guards against null; whitespace is caught by the base class.
        var store = CreateStore();
        store.SetTokens(new OAuthTokens("Bearer", "   ", null, null));
        await ((ITokenStore)store).InitAsync("user1", "session1");
        Assert.Null(await ((ITokenStore)store).GetTokensAsync());
    }

    [Fact]
    public async Task GetTokensAsync_ReturnsToken_WhenNoExpirySet() {
        var store = CreateStore();
        store.SetTokens(new OAuthTokens("Bearer", "access-token", null, null));
        await ((ITokenStore)store).InitAsync("user1", "session1");

        var result = await ((ITokenStore)store).GetTokensAsync();

        Assert.NotNull(result);
        Assert.Equal("access-token", result.Value.AccessToken);
    }

    [Fact]
    public async Task GetTokensAsync_ReturnsToken_WhenNotExpired() {
        var now = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var store = CreateStore(new FixedTimeProvider(now));
        store.SetTokens(new OAuthTokens("Bearer", "access-token", null, now.AddHours(1)));
        await ((ITokenStore)store).InitAsync("user1", "session1");

        var result = await ((ITokenStore)store).GetTokensAsync();

        Assert.NotNull(result);
        Assert.Equal("access-token", result.Value.AccessToken);
    }

    [Fact]
    public async Task GetTokensAsync_ReturnsNull_WhenExpiredAndNoRefreshToken() {
        var now = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var store = CreateStore(new FixedTimeProvider(now));
        store.SetTokens(new OAuthTokens("Bearer", "access-token", null, now.AddSeconds(-1)));
        await ((ITokenStore)store).InitAsync("user1", "session1");

        Assert.Null(await ((ITokenStore)store).GetTokensAsync());
    }

    [Fact]
    public async Task GetTokensAsync_ReturnsToken_WhenRefreshTokenPresent_AndExpiryBeyondBuffer() {
        // Token expiring in 60s: even with the 30s early-expiry buffer applied when a refresh
        // token is present, the token is still considered valid (60 > 30).
        var now = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var store = CreateStore(new FixedTimeProvider(now));
        store.SetTokens(new OAuthTokens("Bearer", "access-token", "refresh-token", now.AddSeconds(60)));
        await ((ITokenStore)store).InitAsync("user1", "session1");

        var result = await ((ITokenStore)store).GetTokensAsync();

        Assert.NotNull(result);
        Assert.Equal("access-token", result.Value.AccessToken);
    }

    [Fact]
    public async Task GetTokensAsync_ReturnsToken_WhenNoRefreshToken_AndWithinBufferWindow() {
        // Without a refresh token the 30s buffer is not applied; a token expiring in 20s
        // is still valid because 20s > 0s.
        var now = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var store = CreateStore(new FixedTimeProvider(now));
        store.SetTokens(new OAuthTokens("Bearer", "access-token", null, now.AddSeconds(20)));
        await ((ITokenStore)store).InitAsync("user1", "session1");

        var result = await ((ITokenStore)store).GetTokensAsync();

        Assert.NotNull(result);
        Assert.Equal("access-token", result.Value.AccessToken);
    }


}
