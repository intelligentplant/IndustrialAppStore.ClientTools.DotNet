﻿using System;

namespace IntelligentPlant.IndustrialAppStore.Authentication {

    /// <summary>
    /// Describes OAuth tokens associated with an Industrial App Store authentication session.
    /// </summary>
    public struct OAuthTokens {

        /// <summary>
        /// The UTC time that the OAuth tokens were obtained at.
        /// </summary>
        public DateTimeOffset UtcCreatedAt { get; }

        /// <summary>
        /// The type of the <see cref="AccessToken"/>.
        /// </summary>
        /// <remarks>
        ///   The <see cref="TokenType"/> is typically the string "bearer".
        /// </remarks>
        public string TokenType { get; }

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
        /// <param name="utcCreatedAt">
        ///   The UTC time that the OAuth tokens were obtained at.
        /// </param>
        /// <param name="tokenType">
        ///   The type of the <paramref name="accessToken"/>. The token type is typically the string "bearer".
        /// </param>
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
        public OAuthTokens(DateTimeOffset utcCreatedAt, string tokenType, string accessToken, string? refreshToken, DateTimeOffset? utcExpiresAt) {
            UtcCreatedAt = utcCreatedAt;
            TokenType = tokenType;
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            RefreshToken = refreshToken;
            UtcExpiresAt = utcExpiresAt;
        }

    }
}
