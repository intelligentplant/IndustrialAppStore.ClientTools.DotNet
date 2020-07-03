using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.IndustrialAppStore.Client.Queries {

    /// <summary>
    /// A request to search for users or user groups.
    /// </summary>
    public class UserOrGroupPrincipalSearchRequest {

        /// <summary>
        /// Maximum page size.
        /// </summary>
        public const int MaxPageSize = 50;

        /// <summary>
        /// The name filter.
        /// </summary>
        [MaxLength(100)]
        public string Filter { get; set; }

        /// <summary>
        /// The page size.
        /// </summary>
        [Range(1, MaxPageSize)]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// The page.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Page { get; set; }

        /// <summary>
        /// Flags if results from trusted external organisations should be included.
        /// </summary>
        public bool IncludeExternalResults { get; set; }

    }
}
