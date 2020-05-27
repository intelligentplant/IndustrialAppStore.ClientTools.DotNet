using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using IntelligentPlant.DataCore.Client.Model.Scripting;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class CreateScriptTagRequest : DataSourceRequest {

        [Required]
        public CreateScriptTagSettings Settings { get; set; }

    }
}
