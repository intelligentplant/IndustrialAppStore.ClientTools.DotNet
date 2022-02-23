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
        [Newtonsoft.Json.JsonProperty("filter")]
        public string Filter { get; set; } = default!;

        /// <summary>
        /// The page size.
        /// </summary>
        [Range(1, MaxPageSize)]
        [Newtonsoft.Json.JsonProperty("pageSize")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// The page.
        /// </summary>
        [Range(1, int.MaxValue)]
        [Newtonsoft.Json.JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        /// Flags if results from trusted external organisations should be included.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("includeExternalResults")]
        public bool IncludeExternalResults { get; set; }

    }
}
