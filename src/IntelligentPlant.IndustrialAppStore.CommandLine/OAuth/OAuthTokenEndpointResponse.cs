using System.Text.Json.Serialization;

#nullable disable warnings

namespace IntelligentPlant.IndustrialAppStore.CommandLine.OAuth {

    /// <summary>
    /// Describes a response from an OAuth token endpoint.
    /// </summary>
    public class OAuthTokenEndpointResponse {

        /// <summary>
        /// The access token.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; }

        /// <summary>
        /// The token type for the access token.
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; }

        /// <summary>
        /// The number of seconds that the access token is valid for.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; }

        /// <summary>
        /// The refresh token, if provided.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; }


        /// <summary>
        /// Creates a new <see cref="OAuthTokenEndpointResponse"/> instance.
        /// </summary>
        /// <param name="accessToken">
        ///   The access token.
        /// </param>
        /// <param name="tokenType">
        ///   The token type for the access token.
        /// </param>
        /// <param name="expiresIn">
        ///   The number of seconds that the access token is valid for.
        /// </param>
        /// <param name="refreshToken">
        ///   The refresh token, if provided.
        /// </param>
        public OAuthTokenEndpointResponse(string accessToken, string tokenType, int? expiresIn, string? refreshToken) {
            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
        }

    }

}
