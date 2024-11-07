namespace IntelligentPlant.IndustrialAppStore.CommandLine.OAuth {

    /// <summary>
    /// Describes OAuth tokens associated with an Industrial App Store authentication session.
    /// </summary>
    public class OAuthTokens {

        /// <summary>
        /// The access token for the authenticated user.
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// The refresh token for the authenticated user. Can be <see langword="null"/>.
        /// </summary>
        public string? RefreshToken { get; }

        /// <summary>
        /// The UTC expiry time for the <see cref="AccessToken"/>. Can be <see langword="null"/> 
        /// if the expiry time is unknown.
        /// </summary>
        public DateTimeOffset? UtcExpiresAt { get; }


        /// <summary>
        /// Creates a new <see cref="OAuthTokens"/> instance.
        /// </summary>
        /// <param name="accessToken">
        ///   The access token.
        /// </param>
        /// <param name="refreshToken">
        ///   The refresh token.
        /// </param>
        /// <param name="utcExpiresAt">
        ///   The UTC expiry time for the <paramref name="accessToken"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="accessToken"/> is <see langword="null"/>.
        /// </exception>
        public OAuthTokens(string accessToken, string? refreshToken, DateTimeOffset? utcExpiresAt) {
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            RefreshToken = refreshToken;
            UtcExpiresAt = utcExpiresAt;
        }

    }
}
