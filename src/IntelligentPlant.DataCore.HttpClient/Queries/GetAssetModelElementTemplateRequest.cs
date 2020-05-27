using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// A request to retrieve an asset model element template.
    /// </summary>
    public class GetAssetModelElementTemplateRequest : DataSourceRequest {

        /// <summary>
        /// The template ID.
        /// </summary>
        [Required]
        public string Id { get; set; }

    }

}
