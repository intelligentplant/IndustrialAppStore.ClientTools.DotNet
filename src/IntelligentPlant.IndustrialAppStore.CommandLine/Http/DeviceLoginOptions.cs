using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.IndustrialAppStore.CommandLine.Http {

    /// <summary>
    /// Options to use when logging in to the Industrial App Store using the device authorization 
    /// grant.
    /// </summary>
    public class DeviceLoginOptions {

        /// <summary>
        /// The device authorization endpoint.
        /// </summary>
        [Required]
        public Uri DeviceAuthorizationEndpoint { get; set; } = default!;

        /// <summary>
        /// The token endpoint.
        /// </summary>
        [Required]
        public Uri TokenEndpoint { get; set; } = default!;

        /// <summary>
        /// The OAuth 2.0 client ID.
        /// </summary>
        [Required]
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// The OAuth 2.0 client secret, if one has been created.
        /// </summary>
        public string? ClientSecret { get; set; }

        /// <summary>
        /// The additional scopes to request when logging in.
        /// </summary>
        /// <remarks>
        ///   Scopes specified here are in addition to the default scopes configured in the app's 
        ///   registration.
        /// </remarks>
        public IEnumerable<string>? Scopes { get; set; }

        /// <summary>
        /// A delegate that will be invoked when a new device authorization request is created.
        /// </summary>
        [Required]
        public DeviceAuthorizationRequestCreatedDelegate OnRequestCreated { get; set; } = default!;

    }
}
