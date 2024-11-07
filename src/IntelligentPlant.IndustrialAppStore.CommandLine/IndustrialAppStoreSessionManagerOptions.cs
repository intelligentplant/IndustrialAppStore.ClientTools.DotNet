using System.ComponentModel.DataAnnotations;

using IntelligentPlant.IndustrialAppStore.Client;

namespace IntelligentPlant.IndustrialAppStore.CommandLine {

    /// <summary>
    /// Options for <see cref="IndustrialAppStoreSessionManager"/>.
    /// </summary>
    public class IndustrialAppStoreSessionManagerOptions {

        /// <summary>
        /// The base URL for the Industrial App Store.
        /// </summary>
        [Required]
        public Uri IndustrialAppStoreUrl { get; set; } = new Uri(IndustrialAppStoreHttpClientDefaults.AppStoreUrl);

        /// <summary>
        /// The base URL for the Industrial App Store Data API.
        /// </summary>
        [Required]
        public Uri DataCoreUrl { get; set; } = new Uri(IndustrialAppStoreHttpClientDefaults.DataCoreUrl);

        /// <summary>
        /// The client ID for the application.
        /// </summary>
        [Required]
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// The client secret for the application.
        /// </summary>
        /// <remarks>
        ///   If a client secret has been generated for the application, it must be provided or 
        ///   device authorization and token endpoint requests will be rejected.
        /// </remarks>
        public string? ClientSecret { get; set; }

        /// <summary>
        /// Additional scopes to request when authenticating the user, in addition to the default 
        /// scopes registered for the app.
        /// </summary>
        public string[]? Scopes { get; set; }

        /// <summary>
        /// The path to the folder where tokens will be stored.
        /// </summary>
        /// <remarks>
        /// 
        /// <para>
        ///   If <see cref="TokenPath"/> is <see langword="null"/>, tokens will not be persisted 
        ///   and a new authentication session must be created each time the application runs.
        /// </para>
        /// 
        /// <para>
        ///   If a relative path is specified, it will be made absolute relative to <see cref="Environment.SpecialFolder.LocalApplicationData"/> 
        ///   if <see cref="Environment.UserInteractive"/> is <see langword="true"/>, and relative 
        ///   to <see cref="Environment.SpecialFolder.CommonApplicationData"/> otherwise.
        /// </para>
        /// 
        /// </remarks>
        public string? TokenPath { get; set; }

    }
}
