using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelligentPlant.IndustrialAppStore.Client.Queries {
    public class UserOrGroupPrincipalSearchRequest {

        /// <summary>
        /// Maximum page size.
        /// </summary>
        public const int MaxPageSize = 50;

        [MaxLength(100)]
        public string Filter { get; set; }

        [Range(1, MaxPageSize)]
        public int PageSize { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; }

        public bool IncludeExternalResults { get; set; }

    }
}
