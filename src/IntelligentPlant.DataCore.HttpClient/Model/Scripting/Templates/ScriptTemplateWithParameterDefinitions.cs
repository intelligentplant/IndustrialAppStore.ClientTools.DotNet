using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model.Scripting.Templates {

    /// <summary>
    /// An extended version of <see cref="ScriptTemplate"/> that includes details about the parameters 
    /// defined on the template.
    /// </summary>
    public class ScriptTemplateWithParameterDefinitions: ScriptTemplate {

        /// <summary>
        /// Gets or sets the parameters that are used in the template.
        /// </summary>
        public IEnumerable<ScriptTemplateParameterDefinition>? Parameters { get; set; }

    }
}
