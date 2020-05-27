using Microsoft.AspNetCore.Http;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Contains constants and properties used by Industrial App Store authentication.
    /// </summary>
    public static class IndustrialAppStoreAuthenticationDefaults {

        /// <summary>
        /// The authentication scheme name.
        /// </summary>
        public const string AuthenticationScheme = "IndustrialAppStore";

        /// <summary>
        /// The authentication scheme display name.
        /// </summary>
        internal static string DisplayName => Resources.AuthDefaults_DisplayName;

        /// <summary>
        /// The OAuth scope for accessing user information.
        /// </summary>
        public const string UserInfoScope = "UserInfo";

        /// <summary>
        /// The OAuth scope for reading real-time and event data.
        /// </summary>
        public const string DataReadScope = "DataRead";

        /// <summary>
        /// The OAuth scope for writing real-time and event data.
        /// </summary>
        public const string DataWriteScope = "DataWrite";

        /// <summary>
        /// The OAuth scope for performing billing requests.
        /// </summary>
        public const string BillingScope = "AccountDebit";

        /// <summary>
        /// The default authorization callback path.
        /// </summary>
        internal static PathString DefaultCallbackPath => new PathString("/auth/signin-ip");

        /// <summary>
        /// The claim type that specifies an Industrial App Store user's profile picture.
        /// </summary>
        public const string PictureClaimType = "urn:ias:picture";

        /// <summary>
        /// The claim type that specifies an Industrial App Store user's organisation ID.
        /// </summary>
        public const string OrgIdentifierClaimType = "urn:ias:org_id";

        /// <summary>
        /// The claim type that specifies an Industrial App Store user's organisation name.
        /// </summary>
        public const string OrgNameClaimType = "urn:ias:org_name";

        /// <summary>
        /// The token name for the OAuth access token.
        /// </summary>
        internal const string AccessTokenName = "access_token";

        /// <summary>
        /// The token name for the OAuth refresh token.
        /// </summary>
        internal const string RefreshTokenName = "refresh_token";

        /// <summary>
        /// The token name for the OAuth expires-at token.
        /// </summary>
        internal const string ExpiresAtTokenName = "expires_at";

        /// <summary>
        /// The token name for the token type.
        /// </summary>
        internal const string TokenTypeTokenName = "token_type";

    }
}
