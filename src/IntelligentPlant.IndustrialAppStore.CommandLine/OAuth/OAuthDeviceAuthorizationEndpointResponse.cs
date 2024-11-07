using System.Text.Json.Serialization;

#nullable disable warnings

namespace IntelligentPlant.IndustrialAppStore.CommandLine.OAuth {

    /// <summary>
    /// Describes a response from an OAuth device authorization endpoint.
    /// </summary>
    public class OAuthDeviceAuthorizationEndpointResponse {

        /// <summary>
        /// The device code.
        /// </summary>
        [JsonPropertyName("device_code")]
        public string DeviceCode { get; }

        /// <summary>
        /// The user code.
        /// </summary>
        [JsonPropertyName("user_code")]
        public string UserCode { get; }

        /// <summary>
        /// The verification URI.
        /// </summary>
        [JsonPropertyName("verification_uri")]
        public Uri VerificationUri { get; }

        /// <summary>
        /// The number of seconds that the device code is valid for.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; }

        /// <summary>
        /// The number of seconds that the client should wait before polling the token endpoint.
        /// </summary>
        /// <remarks>
        ///   A <see langword="null"/> value should be treated as 5 seconds.
        /// </remarks>
        [JsonPropertyName("interval")]
        public int? Interval { get; }


        /// <summary>
        /// Creates a new <see cref="OAuthDeviceAuthorizationEndpointResponse"/> instance.
        /// </summary>
        /// <param name="deviceCode">
        ///   The device code.
        /// </param>
        /// <param name="userCode">
        ///   The user code.
        /// </param>
        /// <param name="verificationUri">
        ///   The verification URI.
        /// </param>
        /// <param name="expiresIn">
        ///   The number of seconds that the device code is valid for.
        /// </param>
        /// <param name="interval">
        ///   The number of seconds that the client should wait before polling the token endpoint.
        /// </param>
        public OAuthDeviceAuthorizationEndpointResponse(string deviceCode, string userCode, Uri verificationUri, int expiresIn, int? interval) {
            DeviceCode = deviceCode;
            UserCode = userCode;
            VerificationUri = verificationUri;
            ExpiresIn = expiresIn;
            Interval = interval;
        }

    }

}
