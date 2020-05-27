using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IntelligentPlant.DataCore.Client.Model.Scripting.Templates;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class CreateTemplatesScriptTagRequest : DataSourceRequest {

        [Required]
        public CreateTemplatedScriptTagSettings Settings { get; set; }

    }
}
