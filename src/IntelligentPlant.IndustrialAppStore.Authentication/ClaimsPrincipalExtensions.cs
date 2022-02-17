using System.Security.Claims;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Extensions for <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public static class ClaimsPrincipalExtensions {

        /// <summary>
        /// Gets the user ID for the <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="principal">
        ///   The <see cref="ClaimsPrincipal"/>.
        /// </param>
        /// <returns>
        ///   The value of the principal's <see cref="ClaimTypes.NameIdentifier"/> 
        ///   claim, or <see langword="null"/> if the claim was not found.
        /// </returns>
        public static string? GetUserId(this ClaimsPrincipal principal) {
            return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }


        /// <summary>
        /// Gets the user name for the <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="principal">
        ///   The <see cref="ClaimsPrincipal"/>.
        /// </param>
        /// <returns>
        ///   The value of the principal's <see cref="ClaimTypes.Name"/> 
        ///   claim, or <see langword="null"/> if the claim was not found.
        /// </returns>
        public static string? GetUserName(this ClaimsPrincipal principal) {
            return principal?.FindFirst(ClaimTypes.Name)?.Value;
        }


        /// <summary>
        /// Gets the organisation name for the <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="principal">
        ///   The <see cref="ClaimsPrincipal"/>.
        /// </param>
        /// <returns>
        ///   The value of the principal's <see cref="IndustrialAppStoreAuthenticationDefaults.OrgNameClaimType"/> 
        ///   claim, or <see langword="null"/> if the claim was not found.
        /// </returns>
        public static string? GetOrganisationName(this ClaimsPrincipal principal) {
            return principal?.FindFirst(IndustrialAppStoreAuthenticationDefaults.OrgNameClaimType)?.Value;
        }


        /// <summary>
        /// Gets the organisation identifier for the <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="principal">
        ///   The <see cref="ClaimsPrincipal"/>.
        /// </param>
        /// <returns>
        ///   The value of the principal's <see cref="IndustrialAppStoreAuthenticationDefaults.OrgIdentifierClaimType"/> 
        ///   claim, or <see langword="null"/> if the claim was not found.
        /// </returns>
        public static string? GetOrganisationId(this ClaimsPrincipal principal) {
            return principal?.FindFirst(IndustrialAppStoreAuthenticationDefaults.OrgIdentifierClaimType)?.Value;
        }


        /// <summary>
        /// Gets the profile picture IRL for the <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="principal">
        ///   The <see cref="ClaimsPrincipal"/>.
        /// </param>
        /// <returns>
        ///   The value of the principal's <see cref="IndustrialAppStoreAuthenticationDefaults.PictureClaimType"/> 
        ///   claim, or <see langword="null"/> if the claim was not found.
        /// </returns>
        public static string? GetProfilePictureUrl(this ClaimsPrincipal principal) {
            return principal?.FindFirst(IndustrialAppStoreAuthenticationDefaults.PictureClaimType)?.Value;
        }


        /// <summary>
        /// Gets the app session ID for the <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="principal">
        ///   The <see cref="ClaimsPrincipal"/>.
        /// </param>
        /// <returns>
        ///   The value of the principal's <see cref="IndustrialAppStoreAuthenticationDefaults.AppSessionIdClaimType"/> 
        ///   claim, or <see langword="null"/> if the claim was not found.
        /// </returns>
        public static string? GetSessionId(this ClaimsPrincipal principal) { 
            return principal?.FindFirst(IndustrialAppStoreAuthenticationDefaults.AppSessionIdClaimType)?.Value;
        }

    }
}
