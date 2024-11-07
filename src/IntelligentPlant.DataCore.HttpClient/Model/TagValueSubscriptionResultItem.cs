namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes the subscription result for a single tag in a <see cref="TagValueSubscriptionResult"/>.
    /// </summary>
    public class TagValueSubscriptionResultItem {

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        public string TagName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the registration status.
        /// </summary>
        public RegistrationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a flag specifying whether or not the calling user is authorised to access the tag.
        /// </summary>
        public bool IsAuthorized { get; set; }

        /// <summary>
        /// Gets or sets the registration messages for the tag.
        /// </summary>
        public IList<string>? Messages { get; set; }

        /// <summary>
        /// Gets or sets the value of the tag when the subscription was created.
        /// </summary>
        public TagValue? Value { get; set; }


        /// <summary>
        /// Describes the registration status of a tag subscription.
        /// </summary>
        [Flags]
        public enum RegistrationStatus {
            /// <summary>
            /// Registration status is unknown.
            /// </summary>
            Unknown = 1,
            /// <summary>
            /// Registration succeeded.
            /// </summary>
            Success = 2,
            /// <summary>
            /// Registration failed.
            /// </summary>
            Failure = 4,
            /// <summary>
            /// Indicates that the current value is temporary and may change if another subscription attempt is made.
            /// </summary>
            Retry = 1024,
            /// <summary>
            /// Indicates that registration failed due to the current state of the application but may succeed in future (e.g. due to lost connectivity to a data source).
            /// </summary>
            TemporaryFailure = Failure | Retry
        }

    }
}
