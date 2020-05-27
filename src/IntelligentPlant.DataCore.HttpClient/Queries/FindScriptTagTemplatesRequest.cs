using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {

    /// <summary>
    /// A request to retrieve script tag templates for a data source and script engine.
    /// </summary>
    public class FindScriptTagTemplatesRequest : DataSourceRequest {

        [Required]
        [MaxLength(100)]
        public string ScriptEngineId { get; set; }

    }
}
