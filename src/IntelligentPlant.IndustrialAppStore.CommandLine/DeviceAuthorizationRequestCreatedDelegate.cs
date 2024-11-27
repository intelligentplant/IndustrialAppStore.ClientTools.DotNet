namespace IntelligentPlant.IndustrialAppStore.CommandLine {

    /// <summary>
    /// Invoked when a device authorization request is created.
    /// </summary>
    /// <param name="details">
    ///   The details of the device authorization request.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///   A <see cref="ValueTask"/> that completes when the operation has completed.
    /// </returns>
    /// <remarks>
    ///   A <see cref="DeviceAuthorizationRequestCreatedDelegate"/> delegate is typically used to 
    ///   notify the user that they must visit the Industrial App Store device verification URL to 
    ///   approve a login request.
    /// </remarks>

    public delegate ValueTask DeviceAuthorizationRequestCreatedDelegate(PendingDeviceAuthorization details, CancellationToken cancellationToken);

}
