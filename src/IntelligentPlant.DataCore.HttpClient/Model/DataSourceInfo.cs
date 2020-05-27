namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes a Data Core data source.
    /// </summary>
    public class DataSourceInfo : ComponentInfo {

        /// <summary>
        /// Gets or sets the driver features support by the data source.
        /// </summary>
        public DataSourceDriverFeatures SupportedFeatures { get; set; }

    }

}
