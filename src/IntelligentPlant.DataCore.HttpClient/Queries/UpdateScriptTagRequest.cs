using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using IntelligentPlant.DataCore.Client.Model.Scripting;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class UpdateScriptTagRequest : DataSourceRequest {

        [Required]
        public string ScriptTagId { get; set; }

        public ScriptTagSettings Settings { get; set; }

    }
}
