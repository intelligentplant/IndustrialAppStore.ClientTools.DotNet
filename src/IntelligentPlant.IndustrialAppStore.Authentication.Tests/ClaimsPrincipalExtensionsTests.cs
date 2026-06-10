using System.Security.Claims;

namespace IntelligentPlant.IndustrialAppStore.Authentication.Tests;

public class ClaimsPrincipalExtensionsTests {

    private static ClaimsPrincipal PrincipalWith(params (string Type, string Value)[] claims) {
        var identity = new ClaimsIdentity(claims.Select(c => new Claim(c.Type, c.Value)));
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public void GetUserId_ReturnsNameIdentifierClaim() {
        var principal = PrincipalWith((ClaimTypes.NameIdentifier, "user-123"));
        Assert.Equal("user-123", principal.GetUserId());
    }

    [Fact]
    public void GetUserId_ReturnsNull_WhenClaimAbsent() {
        Assert.Null(new ClaimsPrincipal().GetUserId());
    }

    [Fact]
    public void GetUserName_ReturnsNameClaim() {
        var principal = PrincipalWith((ClaimTypes.Name, "Jane Smith"));
        Assert.Equal("Jane Smith", principal.GetUserName());
    }

    [Fact]
    public void GetUserName_ReturnsNull_WhenClaimAbsent() {
        Assert.Null(new ClaimsPrincipal().GetUserName());
    }

    [Fact]
    public void GetOrganisationName_ReturnsOrgNameClaim() {
        var principal = PrincipalWith((IndustrialAppStoreAuthenticationDefaults.OrgNameClaimType, "Acme Corp"));
        Assert.Equal("Acme Corp", principal.GetOrganisationName());
    }

    [Fact]
    public void GetOrganisationName_ReturnsNull_WhenClaimAbsent() {
        Assert.Null(new ClaimsPrincipal().GetOrganisationName());
    }

    [Fact]
    public void GetOrganisationId_ReturnsOrgIdClaim() {
        var principal = PrincipalWith((IndustrialAppStoreAuthenticationDefaults.OrgIdentifierClaimType, "org-456"));
        Assert.Equal("org-456", principal.GetOrganisationId());
    }

    [Fact]
    public void GetOrganisationId_ReturnsNull_WhenClaimAbsent() {
        Assert.Null(new ClaimsPrincipal().GetOrganisationId());
    }

    [Fact]
    public void GetProfilePictureUrl_ReturnsPictureClaim() {
        var principal = PrincipalWith((IndustrialAppStoreAuthenticationDefaults.PictureClaimType, "https://example.com/pic.jpg"));
        Assert.Equal("https://example.com/pic.jpg", principal.GetProfilePictureUrl());
    }

    [Fact]
    public void GetProfilePictureUrl_ReturnsNull_WhenClaimAbsent() {
        Assert.Null(new ClaimsPrincipal().GetProfilePictureUrl());
    }

    [Fact]
    public void GetSessionId_ReturnsSessionIdClaim() {
        var principal = PrincipalWith((IndustrialAppStoreAuthenticationDefaults.AppSessionIdClaimType, "sess-abc"));
        Assert.Equal("sess-abc", principal.GetSessionId());
    }

    [Fact]
    public void GetSessionId_ReturnsNull_WhenClaimAbsent() {
        Assert.Null(new ClaimsPrincipal().GetSessionId());
    }

}
