using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class FindTagsRequest : DataSourceRequest {

        [Required]
        public TagSearchFilter Filter { get; set; }

    }
}
