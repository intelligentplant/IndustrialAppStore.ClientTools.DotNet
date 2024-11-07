namespace IntelligentPlant.IndustrialAppStore.CommandLine.OAuth {

    /// <summary>
    /// Describes an exception that is raised due to an error response being received from an 
    /// OAuth endpoint.
    /// </summary>
    public class OAuthException : ApplicationException {

        /// <summary>
        /// The OAuth error response, if available.
        /// </summary>
        public OAuthErrorResponse? OAuthError { get; }


        /// <summary>
        /// Creates a new <see cref="OAuthException"/> instance using the specified <see cref="OAuthErrorResponse"/>.
        /// </summary>
        /// <param name="error">
        ///   The OAuth error response.
        /// </param>
        public OAuthException(OAuthErrorResponse? error) : this(error?.Error, error!) { }


        /// <summary>
        /// Creates a new <see cref="OAuthException"/> instance using the specified message and 
        /// <see cref="OAuthErrorResponse"/>.
        /// </summary>
        /// <param name="message">
        ///   The error message.
        /// </param>
        /// <param name="error">
        ///   The OAuth error response.
        /// </param>
        public OAuthException(string? message, OAuthErrorResponse? error) : base(message) {
            OAuthError = error;
        }


        /// <summary>
        /// Creates a new <see cref="OAuthException"/> instance using the specified message.
        /// </summary>
        /// <param name="message">
        ///   The error message.
        /// </param>
        public OAuthException(string? message) : base(message) { }

    }
}
