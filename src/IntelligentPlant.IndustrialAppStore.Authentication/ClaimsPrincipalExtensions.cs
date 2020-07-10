using System.Security.Claims;

namespace IntelligentPlant.IndustrialAppStore.Authentication {
    public static class ClaimsPrincipalExtensions {

        public static string GetOrganisationName(this ClaimsPrincipal principal) {
            return principal?.FindFirst(IndustrialAppStoreAuthenticationDefaults.OrgNameClaimType)?.Value;
        }


        public static string GetOrganisationId(this ClaimsPrincipal principal) {
            return principal?.FindFirst(IndustrialAppStoreAuthenticationDefaults.OrgIdentifierClaimType)?.Value;
        }


        public static string GetProfilePictureUrl(this ClaimsPrincipal principal) {
            return principal?.FindFirst(IndustrialAppStoreAuthenticationDefaults.PictureClaimType)?.Value;
        }

    }
}
