using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    ///  Describes a digital or enumerated state for a tag.
    /// </summary>
    public class TagDigitalState {

        /// <summary>
        /// Gets or sets the state name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for the state.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value for the state.
        /// </summary>
        public int Value { get; set; }

    }
}
