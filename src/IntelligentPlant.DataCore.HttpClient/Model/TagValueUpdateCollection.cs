using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a collection of tag value updates to be written to a data source tag.
    /// </summary>
    public class TagValueUpdateCollection {

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        [Required]
        public string TagName { get; set; }

        /// <summary>
        /// The sample values.
        /// </summary>
        private ICollection<TagValueUpdate> _values = new List<TagValueUpdate>();

        /// <summary>
        /// Gets or sets the sample values.
        /// </summary>
        [Required]
        [MinLength(1)]
        public ICollection<TagValueUpdate> Values {
            get { return _values; }
            set { _values = new List<TagValueUpdate>(value); }
        }


        /// <summary>
        /// Creates a new <see cref="TagValueUpdateCollection"/> object.
        /// </summary>
        public TagValueUpdateCollection() {
            Values = new List<TagValueUpdate>();
        }

    }
}
