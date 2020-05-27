namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a simplified type for a driver setting or a tag property definition, to help 
    /// applications to infer what sort of editor they should use for a given setting.
    /// </summary>
    public enum SimpleType {
        /// <summary>
        /// The simplified type for the setting is unknown or could not be determined.
        /// </summary>
        unknown,
        /// <summary>
        /// The setting should be edited as text.
        /// </summary>
        text,
        /// <summary>
        /// The setting should be edited as long-form text e.g. by using a TEXTAREA element in an HTML page.
        /// </summary>
        longtext,
        /// <summary>
        /// The setting is an integral numeric type.
        /// </summary>
        integer,
        /// <summary>
        /// The setting is a floating point numeric type.
        /// </summary>
        @float,
        /// <summary>
        /// The setting is a Boolean type.
        /// </summary>
        boolean,
        /// <summary>
        /// The setting is a DateTime type.
        /// </summary>
        datetime,
        /// <summary>
        /// The setting is a TimeSpan type.
        /// </summary>
        timespan,
        /// <summary>
        /// The setting is a URI type.
        /// </summary>
        uri,
        /// <summary>
        /// The setting is an enumeration type.
        /// </summary>
        @enum,
        /// <summary>
        /// The setting is a CRON schedule.
        /// </summary>
        cron,
        /// <summary>
        /// The setting is a reference to a historian tag.
        /// </summary>
        tagReference
    }
}
