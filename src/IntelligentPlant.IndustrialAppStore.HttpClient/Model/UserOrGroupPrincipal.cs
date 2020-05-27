
namespace IntelligentPlant.IndustrialAppStore.Client.Model {

    /// <summary>
    /// Describes an Industrial App Store user or group principal.
    /// </summary>
    public class UserOrGroupPrincipal {

        /// <summary>
        /// The principal name or unique identifier.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The principal's display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The principal's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The principal's picture URL.
        /// </summary>
        public string PicUrl { get; set; }

        /// <summary>
        /// The name of the organisation that the principal belongs to.
        /// </summary>
        public string Org { get; set; }

        /// <summary>
        /// A flag that is <see langword="true"/> if the principal is not from the 
        /// caller's organisation.
        /// </summary>
        public bool External { get; set; }

    }
}
