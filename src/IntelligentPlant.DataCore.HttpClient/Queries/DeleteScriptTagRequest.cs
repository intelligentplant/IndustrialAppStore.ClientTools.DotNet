using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class DeleteScriptTagRequest : DataSourceRequest {

        [Required]
        public string ScriptTagId { get; set; } = default!;

    }
}
