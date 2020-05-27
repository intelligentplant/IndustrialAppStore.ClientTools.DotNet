using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// A request to retrieve detailed information about a script tag template.
    /// </summary>
    public class GetScriptTagTemplateRequest : DataSourceRequest {

        [Required]
        [MaxLength(100)]
        public string ScriptEngineId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TemplateId { get; set; }

    }
}
