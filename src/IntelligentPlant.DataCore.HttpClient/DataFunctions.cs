
namespace IntelligentPlant.DataCore.Client {
    /// <summary>
    /// Defines data functions that can be used when requesting tag values from Data Core.
    /// </summary>
    public static class DataFunctions {

        /// <summary>
        /// Requests the mean aggregated tag value at each specified sample interval.
        /// </summary>
        public const string Average = "AVG";

        /// <summary>
        /// Number of raw samples in a sample interval.
        /// </summary>
        public const string Count = "COUNT";

        /// <summary>
        /// Signed difference between the first and last value in each specified sample interval.
        /// </summary>
        public const string Delta = "DELTA";

        /// <summary>
        /// Requests an interpolated tag value at each specified sample interval.
        /// </summary>
        public const string Interpolated = "INTERP";

        /// <summary>
        /// Requests the minimum aggregated tag value at each specified sample interval.
        /// </summary>
        public const string Minimum = "MIN";

        /// <summary>
        /// Requests the maximum aggregated tag value at each specified sample interval.
        /// </summary>
        public const string Maximum = "MAX";

        /// <summary>
        /// Percentage of time that a tag has good-quality values.
        /// </summary>
        public const string PercentGood = "PERCENTGOOD";

        /// <summary>
        /// Percentage of time that a tag has bad-quality values.
        /// </summary>
        public const string PercentBad = "PERCENTBAD";

        /// <summary>
        /// Requests a trend-friendly aggregation of data.
        /// </summary>
        public const string Plot = "PLOT";

        /// <summary>
        /// Absolute difference between the minimum and maximum value in each specified sample interval.
        /// </summary>
        public const string Range = "RANGE";

        /// <summary>
        /// Requests the  raw historical values recorded between the query start nd end time.
        /// </summary>
        public const string Raw = "RAW";

        /// <summary>
        /// Standard deviation.
        /// </summary>
        public const string StdDev = "STDDEV";

        

        

        /// <summary>
        /// Requests the current tag value.
        /// </summary>
        public const string CurrentValue = "NOW";

    }
}

