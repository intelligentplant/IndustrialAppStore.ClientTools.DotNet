namespace IntelligentPlant.IndustrialAppStore.CommandLine {

    /// <summary>
    /// Constants used by the Industrial App Store CLI services.
    /// </summary>
    internal static class Constants {

        /// <summary>
        /// The path to the device authorization endpoint.
        /// </summary>
        public const string DeviceAuthorizationEndpoint = "/AuthorizationServer/OAuth/AuthorizeDevice";

        /// <summary>
        /// The path to the token endpoint.
        /// </summary>
        public const string TokenEndpoint = "/AuthorizationServer/OAuth/Token";

        /// <summary>
        /// The grant type for the device authorization flow.
        /// </summary>
        public const string DeviceCodeGrantType = "urn:ietf:params:oauth:grant-type:device_code";

    }
}
