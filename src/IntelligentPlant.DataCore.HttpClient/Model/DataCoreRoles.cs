namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Provides constants defining standard Data Core role names.
    /// </summary>
    public static class DataCoreRoles {

        /// <summary>
        /// Gives a caller full permissions on a Data Core component such as a data source.
        /// </summary>
        public const string Administrator = "Administrator";

        /// <summary>
        /// Gives a caller read-only permissions to data such as tag values, events, and annotations.
        /// </summary>
        public const string Read = "Read";

        /// <summary>
        /// Gives a caller permission to write tag values to a data source.
        /// </summary>
        public const string WriteTagValues = "WriteTagValues";

        /// <summary>
        /// Gives a caller permission to create, update and delete annotations for a data source.
        /// </summary>
        public const string WriteAnnotations = "WriteAnnotations";

        /// <summary>
        /// Gives a caller permission to write events to an event sink.
        /// </summary>
        public const string WriteEvents = "WriteEvents";

        /// <summary>
        /// Gives a caller permission to create, update and delete tags for a data source.
        /// </summary>
        public const string ConfigureTags = "ConfigureTags";

        /// <summary>
        /// Gives a caller permission to create, update and delete script tags for a data source.
        /// </summary>
        public const string ConfigureScriptTags = "ConfigureScriptTags";


        /// <summary>
        /// Concatenates the specified roles into a single string.
        /// </summary>
        /// <param name="roles">
        ///   The roles to concatenate.
        /// </param>
        /// <returns>
        ///   The concatenated roles.
        /// </returns>
        public static string Concat(params string[] roles) => string.Join(",", roles);

    }

}
