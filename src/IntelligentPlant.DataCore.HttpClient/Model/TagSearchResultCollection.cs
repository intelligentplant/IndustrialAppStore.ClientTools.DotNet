using System;
using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a set of tags that were retrieved from a data source as part of a composite tag search.
    /// </summary>
    public class TagSearchResultCollection {

        /// <summary>
        /// Gets or sets the data source for the tags.
        /// </summary>
        public ComponentName DataSourceName { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public IEnumerable<TagSearchResult> Tags { get; set; }

        /// <summary>
        /// Gets or sets an error message to accompany the results if the tag search failed.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Flags if <see cref="Error"/> contains an error message.
        /// </summary>
        public bool HasError { get { return !String.IsNullOrWhiteSpace(Error); } }


        /// <summary>
        /// Creates a new <see cref="TagSearchResultCollection"/> object.
        /// </summary>
        public TagSearchResultCollection() {
            Tags = Array.Empty<TagSearchResult>();
        }

    }
}
