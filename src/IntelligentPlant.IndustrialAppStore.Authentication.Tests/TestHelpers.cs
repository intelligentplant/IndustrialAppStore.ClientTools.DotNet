using Microsoft.Extensions.Options;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Tests;

/// <summary>
/// A TimeProvider that returns a fixed UTC time, allowing deterministic expiry tests.
/// </summary>
internal sealed class FixedTimeProvider : TimeProvider {
    private readonly DateTimeOffset _utcNow;
    public FixedTimeProvider(DateTimeOffset utcNow) => _utcNow = utcNow;
    public override DateTimeOffset GetUtcNow() => _utcNow;
}


/// <summary>
/// A concrete TokenStore for exercising the abstract base class logic.
/// Token state is set via SetTokens() before calling GetTokensAsync().
/// </summary>
internal sealed class TestTokenStore : TokenStore {
    private OAuthTokens? _tokens;

    public TestTokenStore(
        IOptions<IndustrialAppStoreAuthenticationOptions> options,
        HttpClient httpClient,
        TimeProvider timeProvider
    ) : base(options, httpClient, timeProvider) { }

    public void SetTokens(OAuthTokens? tokens) => _tokens = tokens;

    protected override ValueTask InitAsync() => default;

    protected override ValueTask<OAuthTokens?> GetTokensAsync() => new(_tokens);

    protected override ValueTask SaveTokensAsync(OAuthTokens tokens) {
        _tokens = tokens;
        return default;
    }
}
