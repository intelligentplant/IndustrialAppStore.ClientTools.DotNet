using System.ComponentModel.DataAnnotations;

using IntelligentPlant.DataCore.Client.Model.Scripting;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class CreateScriptTagRequest : DataSourceRequest {

        [Required]
        public CreateScriptTagSettings Settings { get; set; }

    }
}
