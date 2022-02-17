using System;

namespace IntelligentPlant.DataCore.Client.Model.Scripting {

    /// <summary>
    /// Describes metadata about a script tag.
    /// </summary>
    public class ScriptTagMetadata {

        /// <summary>
        /// Gets or sets the ID of the script engine that the script tag should use.
        /// </summary>
        public string ScriptEngineId { get; set; } = default!;

        /// <summary>
        /// Gets or sets a flag indicating if the script tag was created using a template.
        /// </summary>
        public bool IsFromTemplate { get; set; }

        /// <summary>
        /// Gets or sets the name of the application that created the tag.
        /// </summary>
        public string ApplicationName { get; set; } = default!;

        /// <summary>
        /// Gets or sets information about the user who owns the tag.
        /// </summary>
        public UserInfo? Owner { get; set; }

        /// <summary>
        /// Gets or sets the user who created the tag.
        /// </summary>
        public UserInfo? Creator { get; set; }

        /// <summary>
        /// Gets or sets the UTC creation time for the tag.
        /// </summary>
        public DateTime UtcCreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the tag.
        /// </summary>
        public UserInfo? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the UTC last modified time for the tag.
        /// </summary>
        public DateTime UtcLastModifiedAt { get; set; }

        /// <summary>
        /// Describes information about a user who owns, created, or updated a script tag.
        /// </summary>
        public class UserInfo {

            /// <summary>
            /// Gets or sets the user ID.
            /// </summary>
            public string Id { get; set; } = default!;

            /// <summary>
            /// Gets or sets the user name.
            /// </summary>
            public string Name { get; set; } = default!;

        }

    }
}
