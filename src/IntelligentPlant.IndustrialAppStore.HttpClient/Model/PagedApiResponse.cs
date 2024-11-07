namespace IntelligentPlant.IndustrialAppStore.Client.Model {
    /// <summary>
    /// A response containing a page of items.
    /// </summary>
    /// <typeparam name="T">
    ///   The item type.
    /// </typeparam>
    public class PagedApiResponse<T> {

        /// <summary>
        /// The page size for the response.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// The items.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = default!;

    }
}
