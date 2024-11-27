namespace IntelligentPlant.IndustrialAppStore.CommandLine {

    /// <summary>
    /// Describes an in-progress device authorization request.
    /// </summary>
    public readonly struct PendingDeviceAuthorization {

        /// <summary>
        /// The verification URI that the user must visit to approve the device authorization request.
        /// </summary>
        public Uri VerificationUri { get; }

        /// <summary>
        /// The code that the user must enter to approve the device authorization request.
        /// </summary>
        public string UserCode { get; }

        /// <summary>
        /// The expiry time for the device authorization request.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; }


        /// <summary>
        /// Creates a new <see cref="PendingDeviceAuthorization"/> instance.
        /// </summary>
        /// <param name="verificationUri">
        ///   The verification URI that the user must visit to approve the device authorization request.
        /// </param>
        /// <param name="userCode">
        ///   The code that the user must enter to approve the device authorization request.
        /// </param>
        /// <param name="expiresAt">
        ///   The expiry time for the device authorization request.
        /// </param>
        public PendingDeviceAuthorization(Uri verificationUri, string userCode, DateTimeOffset expiresAt) {
            VerificationUri = verificationUri;
            UserCode = userCode;
            ExpiresAt = expiresAt;
        }

    }
}
