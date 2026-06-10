namespace IntelligentPlant.IndustrialAppStore.Authentication.Tests;

public class OAuthTokensTests {

    [Fact]
    public void Constructor_SetsAllProperties() {
        var expiry = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
        var tokens = new OAuthTokens("Bearer", "access-token", "refresh-token", expiry);

        Assert.Equal("Bearer", tokens.TokenType);
        Assert.Equal("access-token", tokens.AccessToken);
        Assert.Equal("refresh-token", tokens.RefreshToken);
        Assert.Equal(expiry, tokens.UtcExpiresAt);
    }

    [Fact]
    public void Constructor_ThrowsForNullAccessToken() {
        Assert.Throws<ArgumentNullException>(() => new OAuthTokens("Bearer", null!, null, null));
    }

    [Fact]
    public void Constructor_AllowsNullRefreshToken() {
        var tokens = new OAuthTokens("Bearer", "access-token", null, null);
        Assert.Null(tokens.RefreshToken);
    }

    [Fact]
    public void Constructor_AllowsNullExpiry() {
        var tokens = new OAuthTokens("Bearer", "access-token", null, null);
        Assert.Null(tokens.UtcExpiresAt);
    }

    [Fact]
    public void Constructor_AllowsNullTokenType() {
        var tokens = new OAuthTokens(null!, "access-token", null, null);
        Assert.Null(tokens.TokenType);
    }

}
