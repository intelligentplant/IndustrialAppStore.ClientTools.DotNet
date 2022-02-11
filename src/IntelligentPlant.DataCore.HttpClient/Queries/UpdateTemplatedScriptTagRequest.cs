using System.ComponentModel.DataAnnotations;

using IntelligentPlant.DataCore.Client.Model.Scripting.Templates;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class UpdateTemplatedScriptTagRequest : DataSourceRequest {

        [Required]
        public string ScriptTagId { get; set; } = default!;

        [Required]
        public TemplatedScriptTagSettings Settings { get; set; } = default!;

    }
}
