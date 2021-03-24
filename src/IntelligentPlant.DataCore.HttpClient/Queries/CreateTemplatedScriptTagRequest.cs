using System;
using System.ComponentModel.DataAnnotations;

using IntelligentPlant.DataCore.Client.Model.Scripting.Templates;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class CreateTemplatedScriptTagRequest : DataSourceRequest {

        [Required]
        public CreateTemplatedScriptTagSettings Settings { get; set; }

    }


    [Obsolete("Use " + nameof(CreateTemplatedScriptTagRequest) + " instead. This class name contains a typo and exists for backwards-compatibility but will be removed in the future.", false)]
    public class CreateTemplatesScriptTagRequest : CreateTemplatedScriptTagRequest { }
}
