using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model {
    
    /// <summary>
    /// Describes the configuration of a Data Core component.
    /// </summary>
    public class ComponentConfiguration : ComponentInfo {

        /// <summary>
        /// The configuration settings for the component.
        /// </summary>
        public IEnumerable<ComponentConfigurationSetting>? Settings { get; set; }

    }

}
