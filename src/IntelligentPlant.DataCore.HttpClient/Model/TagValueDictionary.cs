namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Dictionary that maps from tag name to tag values.
    /// </summary>
    public class TagValueDictionary : Dictionary<string, IEnumerable<TagValue>> {

        /// <summary>
        /// Creates a new <see cref="TagValueDictionary"/> object that uses case-insensitive indexing.
        /// </summary>
        public TagValueDictionary() : base(StringComparer.OrdinalIgnoreCase) { }


        /// <summary>
        /// Creates a new <see cref="TagValueDictionary"/> that uses the specified index comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use for indexing.</param>
        public TagValueDictionary(IEqualityComparer<string> comparer) : base(comparer) { }

    }


    /// <summary>
    /// Dictionary that maps from tag name to a single snapshot tag value.
    /// </summary>
    public class SnapshotTagValueDictionary : Dictionary<string, TagValue> {

        /// <summary>
        /// Creates a new <see cref="SnapshotTagValueDictionary"/> object that uses case-insensitive indexing.
        /// </summary>
        public SnapshotTagValueDictionary() : base(StringComparer.OrdinalIgnoreCase) { }

        /// <summary>
        /// Creates a new <see cref="SnapshotTagValueDictionary"/> that uses the specified index comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use for indexing.</param>
        public SnapshotTagValueDictionary(IEqualityComparer<string> comparer) : base(comparer) { }


        /// <summary>
        /// Creates a new <see cref="SnapshotTagValueDictionary"/> object containing the specified values.
        /// </summary>
        /// <param name="values">The values to add to the dictionary.</param>
        public SnapshotTagValueDictionary(IDictionary<string, TagValue> values) : this() {
            if (values == null) {
                return;
            }

            foreach (var item in values) {
                this[item.Key] = item.Value;
            }
        }


        /// <summary>
        /// Creates a new <see cref="SnapshotTagValueDictionary"/> object containing the specified values.
        /// </summary>
        /// <param name="values">The values to add to the dictionary.</param>
        /// <param name="comparer">The comparer to use for indexing.</param>
        internal SnapshotTagValueDictionary(IDictionary<string, TagValue> values, IEqualityComparer<string> comparer) : this(comparer) {
            if (values == null) {
                return;
            }

            foreach (var item in values) {
                this[item.Key] = item.Value;
            }
        }

    }


    /// <summary>
    /// Dictionary that maps from tag name to a set of <see cref="HistoricalTagValues"/> for the tag.
    /// </summary>
    public class HistoricalTagValuesDictionary : Dictionary<string, HistoricalTagValues> {

        /// <summary>
        /// Creates a new <see cref="HistoricalTagValuesDictionary"/> object that uses case-insensitive indexing.
        /// </summary>
        public HistoricalTagValuesDictionary() : base(StringComparer.OrdinalIgnoreCase) { }

        /// <summary>
        /// Creates a new <see cref="HistoricalTagValuesDictionary"/> that uses the specified index comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use for indexing.</param>
        public HistoricalTagValuesDictionary(IEqualityComparer<string> comparer) : base(comparer) { }

    }

}
