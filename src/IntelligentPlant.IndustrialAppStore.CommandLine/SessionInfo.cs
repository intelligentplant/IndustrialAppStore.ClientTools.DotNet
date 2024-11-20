namespace IntelligentPlant.IndustrialAppStore.CommandLine {

    /// <summary>
    /// Provides basic information about an Industrial App Store authentication session.
    /// </summary>
    public readonly struct SessionInfo {

        /// <summary>
        /// The expiry time for the session.
        /// </summary>
        public DateTimeOffset? ExpiresAt { get; }

        /// <summary>
        /// Specifies if the session has a refresh token.
        /// </summary>
        public bool HasRefreshToken { get; }


        /// <summary>
        /// Creates a new <see cref="SessionInfo"/> instance.
        /// </summary>
        /// <param name="expiresAt">
        ///   The expiry time for the session.
        /// </param>
        /// <param name="hasRefreshToken">
        ///   Specifies if the session has a refresh token.
        /// </param>
        public SessionInfo(DateTimeOffset? expiresAt, bool hasRefreshToken) {
            ExpiresAt = expiresAt;
            HasRefreshToken = hasRefreshToken;
        }

    }
}
