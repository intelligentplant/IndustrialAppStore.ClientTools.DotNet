﻿using System.ComponentModel.DataAnnotations;

using IntelligentPlant.DataCore.Client.Model.Scripting;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class UpdateScriptTagRequest : DataSourceRequest {

        [Required]
        public string ScriptTagId { get; set; } = default!;

        [Required]
        public ScriptTagSettings Settings { get; set; } = default!;

    }
}
