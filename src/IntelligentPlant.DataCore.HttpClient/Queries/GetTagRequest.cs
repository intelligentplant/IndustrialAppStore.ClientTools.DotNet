using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class GetTagRequest : DataSourceRequest {

        [Required]
        public string TagNameOrId { get; set; }

    }
}
